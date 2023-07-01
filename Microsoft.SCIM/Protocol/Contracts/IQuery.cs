//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IQuery
    {
        IReadOnlyCollection<IFilter> AlternateFilters { get; set; }
        IReadOnlyCollection<string> ExcludedAttributePaths { get; set; }
        IPaginationParameters PaginationParameters { get; set; }
        string Path { get; set; }
        IReadOnlyCollection<string> RequestedAttributePaths { get; set; }

        string Compose();
    }
}