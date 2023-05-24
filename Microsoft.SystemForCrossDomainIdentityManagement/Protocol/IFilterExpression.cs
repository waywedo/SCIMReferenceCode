//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    internal interface IFilterExpression
    {
        IReadOnlyCollection<IFilter> ToFilters();
    }
}