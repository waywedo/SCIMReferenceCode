// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.SCIM
{
    public abstract class ProviderBase : IProvider
    {
        private static readonly Lazy<BulkRequestsFeature> BulkFeatureSupport =
            new(() => BulkRequestsFeature.CreateUnsupportedFeature());

        private static readonly Lazy<IReadOnlyCollection<TypeScheme>> TypeSchema =
            new(() => Array.Empty<TypeScheme>());

        private static readonly Lazy<ServiceConfigurationBase> ServiceConfiguration =
            new(() => new Core2ServiceConfiguration(BulkFeatureSupport.Value, false, true, false, true, false));

        private static readonly Lazy<IReadOnlyCollection<Core2ResourceType>> Types =
            new(() => Array.Empty<Core2ResourceType>());

        public virtual bool AcceptLargeObjects { get; set; }

        public virtual ServiceConfigurationBase Configuration
        {
            get { return ServiceConfiguration.Value; }
        }

        //public virtual IEventTokenHandler EventHandler
        //{
        //    get;
        //    set;
        //}

        public virtual IReadOnlyCollection<IExtension> Extensions
        {
            get { return null; }
        }

        public virtual IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior
        {
            get { return null; }
        }

        public virtual ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequestDeserializationBehavior
        {
            get { return null; }
        }

        public virtual IReadOnlyCollection<Core2ResourceType> ResourceTypes
        {
            get { return Types.Value; }
        }

        public virtual IReadOnlyCollection<TypeScheme> Schema
        {
            get { return TypeSchema.Value; }
        }

        //public virtual Action<IAppBuilder, HttpConfiguration> StartupBehavior
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        public virtual IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior
        {
            get { return null; }
        }

        public abstract Task<Resource> CreateAsync(Resource resource, string correlationIdentifier);

        public virtual Task<Resource> CreateAsync(IRequest<Resource> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            return CreateAsync(request.Payload, request.CorrelationIdentifier);
        }

        public abstract Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier);

        public virtual Task DeleteAsync(IRequest<IResourceIdentifier> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            return DeleteAsync(request.Payload, request.CorrelationIdentifier);
        }

        public virtual Task<BulkResponse2> ProcessAsync(IRequest<BulkRequest2> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Request == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            var operations = request.EnqueueOperations();

            return ProcessAsync(operations);
        }

        public virtual async Task ProcessAsync(IBulkOperationContext operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (!operation.TryPrepare())
            {
                return;
            }

            if (operation.Method == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            if (operation.Operation == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            var response = new BulkResponseOperation(operation.Operation.Identifier)
            {
                Method = operation.Method
            };

            if (operation.Method == HttpMethod.Delete)
            {
                var context = (IBulkOperationContext<IResourceIdentifier>)operation;
                await DeleteAsync(context.Request).ConfigureAwait(false);
                response.Status = HttpStatusCode.NoContent;
            }
            else if (operation.Method == HttpMethod.Get)
            {
                switch (operation)
                {
                    case IBulkOperationContext<IResourceRetrievalParameters> retrievalContext:
                        response.Response = await RetrieveAsync(retrievalContext.Request).ConfigureAwait(false);
                        break;
                    default:
                        IBulkOperationContext<IQueryParameters> queryContext = (IBulkOperationContext<IQueryParameters>)operation;
                        response.Response = await PaginateQueryAsync(queryContext.Request).ConfigureAwait(false);
                        break;
                }
                response.Status = HttpStatusCode.OK;
            }
            else if (operation.Method == ProtocolExtensions.PatchMethod)
            {
                IBulkOperationContext<IPatch> context = (IBulkOperationContext<IPatch>)operation;
                await UpdateAsync(context.Request).ConfigureAwait(false);
                response.Status = HttpStatusCode.OK;
            }
            else if (operation.Method == HttpMethod.Post)
            {
                IBulkOperationContext<Resource> context = (IBulkOperationContext<Resource>)operation;
                Resource output = await CreateAsync(context.Request).ConfigureAwait(false);
                response.Status = HttpStatusCode.Created;
                response.Location = output.GetResourceIdentifier(context.BulkRequest.BaseResourceIdentifier);
            }
            else
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionMethodNotSupportedTemplate,
                    operation.Method);

                response.Response = new ErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Detail = exceptionMessage
                };

                response.Status = HttpStatusCode.BadRequest;
            }

            operation.Complete(response);
        }

        public virtual async Task<BulkResponse2> ProcessAsync(Queue<IBulkOperationContext> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            var result = new BulkResponse2();
            var countFailures = 0;

            while (operations.Any())
            {
                var operation = operations.Dequeue();

                await ProcessAsync(operation).ConfigureAwait(false);

                var addOperation = operation is not IBulkUpdateOperationContext updateOperation || updateOperation.Parent == null;

                if (addOperation)
                {
                    result.AddOperation(operation.Response);
                }

                if (operation.Response.IsError())
                {
                    checked
                    {
                        countFailures++;
                    }
                }

                if (operation.BulkRequest.Payload.FailOnErrors.HasValue
                    && countFailures > operation.BulkRequest.Payload.FailOnErrors.Value)
                {
                    break;
                }
            }

            return result;
        }

        public async Task<QueryResponseBase> PaginateQueryAsync(IRequest<IQueryParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            ////TODO: This is terrible for pagination
            //IReadOnlyCollection<Resource> resources = await QueryAsync(request).ConfigureAwait(false);
            //var result = new QueryResponse(resources);

            //result.TotalResults = result.ItemsPerPage = resources.Count;
            //result.StartIndex = resources.Any() ? 1 : null;

            return await QueryAsync(request.Payload, request.CorrelationIdentifier);
        }

        public virtual Task<QueryResponseBase> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        //public virtual Task<Resource[]> QueryAsync(IRequest<IQueryParameters> request)
        //{
        //    if (request == null)
        //    {
        //        throw new ArgumentNullException(nameof(request));
        //    }

        //    if (request.Payload == null)
        //    {
        //        throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
        //    }

        //    if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
        //    {
        //        throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
        //    }

        //    return QueryAsync(request.Payload, request.CorrelationIdentifier);
        //}

        public virtual Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            throw new NotSupportedException();
        }

        public virtual Task<Resource> ReplaceAsync(IRequest<Resource> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            return ReplaceAsync(request.Payload, request.CorrelationIdentifier);
        }

        public abstract Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier);

        public virtual Task<Resource> RetrieveAsync(IRequest<IResourceRetrievalParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            return RetrieveAsync(request.Payload, request.CorrelationIdentifier);
        }

        public abstract Task UpdateAsync(IPatch patch, string correlationIdentifier);

        public virtual Task UpdateAsync(IRequest<IPatch> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            return UpdateAsync(request.Payload, request.CorrelationIdentifier);
        }
    }
}
