// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleResourceTypeAttributes
    {
        public static AttributeScheme NameAttributeScheme
        {
            get
            {
                return new("name", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_RESOURCE_TYPE_NAME
                };
            }
        }

        public static AttributeScheme EndpointAttributeScheme
        {
            get
            {
                var endpointScheme = new AttributeScheme("endpoint", AttributeDataType.reference, false)
                {
                    Description = SampleConstants.DESCRIPTION_RESOURCE_TYPE_ENDPOINT,
                    Required = true,
                    Mutability = Mutability.readOnly
                };
                endpointScheme.AddReferenceTypes("uri");

                return endpointScheme;
            }
        }

        public static AttributeScheme SchemaAttributeScheme
        {
            get
            {
                var schemaScheme = new AttributeScheme("schema", AttributeDataType.reference, false)
                {
                    Description = SampleConstants.DESCRIPTION_RESOURCE_TYPE_SCHEMA_ATTRIBUTE,
                    Required = true,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
                schemaScheme.AddReferenceTypes("uri");

                return schemaScheme;
            }
        }
    }
}
