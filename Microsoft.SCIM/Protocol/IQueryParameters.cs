//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IQueryParameters : IRetrievalParameters
    {
        IReadOnlyCollection<IFilter> AlternateFilters { get; }
        IPaginationParameters PaginationParameters { get; set; }
    }
}