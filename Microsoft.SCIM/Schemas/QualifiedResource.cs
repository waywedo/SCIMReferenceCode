//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.SCIM.Resources;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class QualifiedResource : Resource
    {
        private const string RESOURCE_SCHEMA_IDENTIFIER_TEMPLATE_SUFFIX = "{0}";
        private string _resourceSchemaIdentifierTemplate;

        protected QualifiedResource(string schemaIdentifier, string resourceSchemaPrefix)
        {
            OnInitialized(schemaIdentifier, resourceSchemaPrefix);
        }

        private string ResourceSchemaPrefix { get; set; }

        public virtual void AddResourceSchemaIdentifier(string resourceTypeName)
        {
            if (TryGetResourceTypeName(out _))
            {
                var typeName = GetType().Name;
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionMultipleQualifiedResourceTypeIdentifiersTemplate,
                    typeName
                );
                throw new InvalidOperationException(errorMessage);
            }

            var schemaIdentifier = string.Format(CultureInfo.InvariantCulture, _resourceSchemaIdentifierTemplate,
                resourceTypeName);

            AddSchema(schemaIdentifier);
        }

        public void OnDeserialized(string schemaIdentifier, string resourceSchemaPrefix)
        {
            OnInitialized(schemaIdentifier, resourceSchemaPrefix);

            var countResourceSchemaIdentifiers = Schemas.Count(
                item => item.StartsWith(ResourceSchemaPrefix, StringComparison.Ordinal)
            );

            if (countResourceSchemaIdentifiers > 1)
            {
                var typeName = GetType().Name;
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    SchemasResources.ExceptionMultipleQualifiedResourceTypeIdentifiersTemplate,
                    typeName
                );

                throw new InvalidOperationException(errorMessage);
            }
        }

        private void OnInitialized(string schemaIdentifier, string resourceSchemaPrefix)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resourceSchemaPrefix))
            {
                throw new ArgumentNullException(nameof(resourceSchemaPrefix));
            }

            ResourceSchemaPrefix = resourceSchemaPrefix;
            _resourceSchemaIdentifierTemplate = ResourceSchemaPrefix + RESOURCE_SCHEMA_IDENTIFIER_TEMPLATE_SUFFIX;
        }

        public virtual bool TryGetResourceTypeName(out string resourceTypeName)
        {
            resourceTypeName = null;

            var resourceSchemaIdentifier = Schemas.SingleOrDefault(
                item => item.StartsWith(ResourceSchemaPrefix, StringComparison.Ordinal)
            );

            if (string.IsNullOrWhiteSpace(resourceSchemaIdentifier))
            {
                return false;
            }

            var buffer = resourceSchemaIdentifier[ResourceSchemaPrefix.Length..];

            if (buffer.Length == 0)
            {
                return false;
            }

            resourceTypeName = buffer;

            return true;
        }
    }
}