// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IResourceQuery
    {
        IReadOnlyCollection<string> Attributes { get; }
        IReadOnlyCollection<string> ExcludedAttributes { get; }
        IReadOnlyCollection<IFilter> Filters { get; }
        IPaginationParameters PaginationParameters { get; }
    }
}
