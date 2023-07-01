// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IProvider
    {
        bool AcceptLargeObjects { get; set; }
        ServiceConfigurationBase Configuration { get; }
        IReadOnlyCollection<IExtension> Extensions { get; }
        IReadOnlyCollection<Core2ResourceType> ResourceTypes { get; }
        IReadOnlyCollection<TypeScheme> Schema { get; }
        Task<Resource> CreateAsync(IRequest<Resource> request);
        Task DeleteAsync(IRequest<IResourceIdentifier> request);
        Task<QueryResponseBase> PaginateQueryAsync(IRequest<IQueryParameters> request);
        Task<Resource> ReplaceAsync(IRequest<Resource> request);
        Task<Resource> RetrieveAsync(IRequest<IResourceRetrievalParameters> request);
        Task UpdateAsync(IRequest<IPatch> request);
        Task<BulkResponse2> ProcessAsync(IRequest<BulkRequest2> request);
    }
}
