// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Microsoft.SCIM
{
    public interface IProviderAdapter<T> where T : Resource
    {
        string SchemaIdentifier { get; }

        Task<Resource> Create(HttpRequest request, Resource resource, string correlationIdentifier);
        Task Delete(HttpRequest request, string identifier, string correlationIdentifier);
        Task<QueryResponseBase> Query(HttpRequest request, IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters, string correlationIdentifier);
        Task<Resource> Replace(HttpRequest request, Resource resource, string correlationIdentifier);
        Task<Resource> Retrieve(HttpRequest request, string identifier, IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths, string correlationIdentifier);
        Task Update(HttpRequest request, string identifier, PatchRequestBase patchRequest,
            string correlationIdentifier);
    }
}
