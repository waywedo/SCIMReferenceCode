//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public sealed class QueryParameters : RetrievalParameters, IQueryParameters
    {
        public QueryParameters(string schemaIdentifier, string path, IFilter filter,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            AlternateFilters = filter.ToCollection();
        }

        public QueryParameters(string schemaIdentifier, string path, IReadOnlyCollection<IFilter> alternateFilters,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            AlternateFilters = alternateFilters ?? throw new ArgumentNullException(nameof(alternateFilters));
        }

        public QueryParameters(string schemaIdentifier, string path, IPaginationParameters paginationParameters)
            : this(schemaIdentifier, path, Array.Empty<IFilter>(), Array.Empty<string>(), Array.Empty<string>())
        {
            PaginationParameters = paginationParameters ?? throw new ArgumentNullException(nameof(paginationParameters));
        }

        [Obsolete("Use QueryParameters(string, string, IFilter, IReadOnlyCollection<string>, IReadOnlyCollection<string>) instead")]
        public QueryParameters(string schemaIdentifier, IFilter filter, IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : this(schemaIdentifier, new SchemaIdentifier(schemaIdentifier).FindPath(), filter, requestedAttributePaths,
                  excludedAttributePaths)
        {
        }

        [Obsolete("Use QueryParameters(string, string, IReadOnlyCollection<IFilter>, IReadOnlyCollection<string>, IReadOnlyCollection<string>) instead")]
        public QueryParameters(string schemaIdentifier, IReadOnlyCollection<IFilter> alternateFilters,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths)
            : this(schemaIdentifier, new SchemaIdentifier(schemaIdentifier).FindPath(), alternateFilters, requestedAttributePaths,
                  excludedAttributePaths)
        {
        }

        public IReadOnlyCollection<IFilter> AlternateFilters { get; }

        public IPaginationParameters PaginationParameters { get; set; }

        public override string ToString()
        {
            return new Query
            {
                AlternateFilters = AlternateFilters,
                RequestedAttributePaths = RequestedAttributePaths,
                ExcludedAttributePaths = ExcludedAttributePaths,
                PaginationParameters = PaginationParameters
            }.Compose();
        }
    }
}