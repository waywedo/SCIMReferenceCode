// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Service
{
    public abstract class ProviderAdapterTemplate<T> : IProviderAdapter<T> where T : Resource
    {
        protected ProviderAdapterTemplate(IProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IProvider Provider { get; set; }

        public abstract string SchemaIdentifier { get; }

        public virtual Task<Resource> Create(HttpRequest request, Resource resource, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var extensions = ReadExtensions();
            var creationRequest = new CreationRequest(request, resource, correlationIdentifier, extensions);

            return Provider.CreateAsync(creationRequest);
        }

        public virtual IResourceIdentifier CreateResourceIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return new ResourceIdentifier()
            {
                Identifier = identifier,
                SchemaIdentifier = SchemaIdentifier
            };
        }

        public virtual Task Delete(HttpRequest request, string identifier, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var extensions = ReadExtensions();
            var resourceIdentifier = CreateResourceIdentifier(identifier);
            var deletionRequest = new DeletionRequest(request, resourceIdentifier, correlationIdentifier, extensions);

            return Provider.DeleteAsync(deletionRequest);
        }

        public virtual string GetPath(HttpRequest request)
        {
            var extensions = ReadExtensions();

            if (extensions != null && extensions.TryGetPath(SchemaIdentifier, out string result))
            {
                return result;
            }

            return new SchemaIdentifier(SchemaIdentifier).FindPath();
        }

        public virtual Task<QueryResponseBase> Query(HttpRequest request,
            IReadOnlyCollection<IFilter> filters, IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths, IPaginationParameters paginationParameters,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var path = GetPath(request);
            var queryParameters = new QueryParameters(SchemaIdentifier, path, filters, requestedAttributePaths, excludedAttributePaths)
            {
                PaginationParameters = paginationParameters
            };
            var extensions = ReadExtensions();
            var queryRequest = new QueryRequest(request, queryParameters, correlationIdentifier, extensions);

            return Provider.PaginateQueryAsync(queryRequest);
        }

        private IReadOnlyCollection<IExtension> ReadExtensions()
        {
            IReadOnlyCollection<IExtension> result;

            try
            {
                result = Provider.Extensions;
            }
            catch (NotImplementedException)
            {
                result = null;
            }

            return result;
        }

        public virtual Task<Resource> Replace(HttpRequest request, Resource resource, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var extensions = ReadExtensions();
            var replaceRequest = new ReplaceRequest(request, resource, correlationIdentifier, extensions);

            return Provider.ReplaceAsync(replaceRequest);
        }

        public virtual Task<Resource> Retrieve(HttpRequest request, string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var path = GetPath(request);
            var retrievalParameters = new ResourceRetrievalParameters(SchemaIdentifier, path, identifier,
                requestedAttributePaths, excludedAttributePaths);
            var extensions = ReadExtensions();
            var retrievalRequest = new RetrievalRequest(request, retrievalParameters, correlationIdentifier, extensions);

            return Provider.RetrieveAsync(retrievalRequest);
        }

        public virtual Task Update(HttpRequest request, string identifier, PatchRequestBase patchRequest,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            var resourceIdentifier = CreateResourceIdentifier(identifier);
            var extensions = ReadExtensions();
            var payload = new Patch
            {
                ResourceIdentifier = resourceIdentifier,
                PatchRequest = patchRequest
            };
            var updateRequest = new UpdateRequest(request, payload, correlationIdentifier, extensions);

            return Provider.UpdateAsync(updateRequest);
        }
    }
}
