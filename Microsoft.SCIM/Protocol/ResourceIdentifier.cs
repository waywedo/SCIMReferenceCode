//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;

namespace Microsoft.SCIM
{
    public sealed class ResourceIdentifier : IResourceIdentifier
    {
        public ResourceIdentifier()
        {
        }

        public ResourceIdentifier(string schemaIdentifier, string resourceIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resourceIdentifier))
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            SchemaIdentifier = schemaIdentifier;
            Identifier = resourceIdentifier;
        }

        public string Identifier { get; set; }

        public string SchemaIdentifier { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not IResourceIdentifier otherIdentifier)
            {
                return false;
            }

            if (!string.Equals(SchemaIdentifier, otherIdentifier.SchemaIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(Identifier, otherIdentifier.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var identifierCode = string.IsNullOrWhiteSpace(Identifier)
                ? 0
                : Identifier.GetHashCode(StringComparison.InvariantCulture);
            var schemaIdentifierCode = string.IsNullOrWhiteSpace(SchemaIdentifier)
                ? 0
                : SchemaIdentifier.GetHashCode(StringComparison.InvariantCulture);
            var result = identifierCode ^ schemaIdentifierCode;

            return result;
        }

        public override string ToString()
        {
            var result = string.Format(CultureInfo.InvariantCulture,
                ProtocolResources.ResourceIdentifierTemplate,
                SchemaIdentifier, Identifier);

            return result;
        }
    }
}