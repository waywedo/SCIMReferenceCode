//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Service.Contracts;
using Newtonsoft.Json;

namespace Microsoft.SCIM.Service
{
    internal class BulkUpdateOperationState : BulkOperationStateBase<IPatch>, IBulkUpdateOperationState
    {
        private readonly List<IBulkCreationOperationContext> _dependencies;

        public BulkUpdateOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkOperationContext<IPatch> context)
            : base(request, operation, context)
        {
            _dependencies = new List<IBulkCreationOperationContext>();
            Dependencies = _dependencies.AsReadOnly();
        }

        public BulkUpdateOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkOperationContext<IPatch> context, IBulkCreationOperationContext parent)
            : this(request, operation, context)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IReadOnlyCollection<IBulkCreationOperationContext> Dependencies { get; }

        public IBulkCreationOperationContext Parent { get; }

        public void AddDependency(IBulkCreationOperationContext dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (Context.State != Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidState);
            }

            _dependencies.Add(dependency);
        }

        public override void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (Context.State != Context.ReceivedState && Context.State != Context.PreparedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            IBulkOperationState<IPatch> completionState;

            if (response.Response is ErrorResponse)
            {
                completionState = Context.FaultedState;
            }
            else
            {
                completionState = Context.ProcessedState;
            }

            if (this == completionState)
            {
                Response = response;
                Context.State = this;

                Parent?.Complete(response);
            }
            else
            {
                completionState.Complete(response);
            }
        }

        private void Fault(HttpStatusCode statusCode, ErrorType? errorType = null)
        {
            var error = new ErrorResponse()
            {
                Status = statusCode
            };

            if (errorType.HasValue)
            {
                error.ErrorType = errorType.Value;
            }

            var response = new BulkResponseOperation(Operation.Identifier)
            {
                Response = error
            };

            Complete(response);
        }

        public override bool TryPrepareRequest(out IRequest<IPatch> request)
        {
            request = null;

            PatchRequest2 patchRequest;

            switch (Operation.Data)
            {
                case PatchRequest2 patchrequest2:
                    patchRequest = patchrequest2;
                    break;
                default:
                    dynamic operationDataJson = JsonConvert.DeserializeObject(Operation.Data.ToString());
                    var patchOperations = operationDataJson.Operations.ToObject<List<PatchOperation2Combined>>();
                    patchRequest = new PatchRequest2(patchOperations);
                    break;
            }
            var patch = new Patch()
            {
                PatchRequest = patchRequest
            };
            var requestBuffer = new UpdateRequest(
                BulkRequest.Request,
                patch,
                BulkRequest.CorrelationIdentifier,
                BulkRequest.Extensions
            );

            Uri resourceIdentifier;

            if (Parent != null)
            {
                if (Parent.Response == null || Parent.Response.Location == null)
                {
                    Fault(HttpStatusCode.NotFound, ErrorType.noTarget);
                    return false;
                }

                resourceIdentifier = Parent.Response.Location;
            }
            else
            {
                if (BulkRequest == null || BulkRequest.BaseResourceIdentifier == null)
                {
                    throw new InvalidOperationException(ServiceResources.ExceptionInvalidState);
                }

                if (Operation.Path == null)
                {
                    Fault(HttpStatusCode.BadRequest);
                    return false;
                }

                resourceIdentifier = new Uri(BulkRequest.BaseResourceIdentifier, Operation.Path);
            }

            if (!UniformResourceIdentifier.TryParse(resourceIdentifier, BulkRequest.Extensions,
                out IUniformResourceIdentifier parsedIdentifier)
                || parsedIdentifier == null || parsedIdentifier.Identifier == null)
            {
                Fault(HttpStatusCode.BadRequest);
                return false;
            }

            requestBuffer.Payload.ResourceIdentifier = parsedIdentifier.Identifier;

            if (Dependencies.Any())
            {
                foreach (IBulkCreationOperationContext dependency in Dependencies)
                {
                    if (dependency.Response == null || dependency.Response.Location == null
                        || !UniformResourceIdentifier.TryParse(dependency.Response.Location, BulkRequest.Extensions,
                        out IUniformResourceIdentifier dependentResourceIdentifier)
                        || dependentResourceIdentifier.Identifier == null
                        || string.IsNullOrWhiteSpace(dependentResourceIdentifier.Identifier.Identifier))
                    {
                        Fault(HttpStatusCode.NotFound, ErrorType.noTarget);
                        return false;
                    }

                    if (!patchRequest.TryFindReference(dependency.Operation.Identifier,
                        out IReadOnlyCollection<OperationValue> references))
                    {
                        Fault(HttpStatusCode.InternalServerError);
                        return false;
                    }

                    foreach (OperationValue value in references)
                    {
                        value.Value = dependentResourceIdentifier.Identifier.Identifier;
                    }
                }
            }

            request = requestBuffer;
            return true;
        }
    }
}
