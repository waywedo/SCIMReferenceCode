// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IResourceQuery
    {
        IReadOnlyCollection<string> Attributes { get; }
        IReadOnlyCollection<string> ExcludedAttributes { get; }
        IReadOnlyCollection<IFilter> Filters { get; }
        IPaginationParameters PaginationParameters { get; }
    }
}
