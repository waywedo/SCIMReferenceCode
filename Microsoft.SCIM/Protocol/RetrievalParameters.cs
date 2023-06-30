//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public abstract class RetrievalParameters : IRetrievalParameters
    {
        protected RetrievalParameters(string schemaIdentifier, string path,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            SchemaIdentifier = schemaIdentifier;
            Path = path;
            RequestedAttributePaths = requestedAttributePaths
                ?? throw new ArgumentNullException(nameof(requestedAttributePaths));
            ExcludedAttributePaths = excludedAttributePaths
                ?? throw new ArgumentNullException(nameof(excludedAttributePaths));
        }

        protected RetrievalParameters(string schemaIdentifier, string path)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            SchemaIdentifier = schemaIdentifier;
            Path = path;
            RequestedAttributePaths = Array.Empty<string>();
            ExcludedAttributePaths = Array.Empty<string>();
        }

        public IReadOnlyCollection<string> ExcludedAttributePaths { get; }

        public string Path { get; }

        public IReadOnlyCollection<string> RequestedAttributePaths { get; }

        public string SchemaIdentifier { get; }

        public override string ToString()
        {
            return new Query
            {
                RequestedAttributePaths = RequestedAttributePaths,
                ExcludedAttributePaths = ExcludedAttributePaths
            }.Compose();
        }
    }
}