// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleServiceProviderConfigAttributes
    {
        public static AttributeScheme DocumentationUriAttributeScheme
        {
            get
            {
                var documentationUriScheme = new AttributeScheme("documentationUri", AttributeDataType.reference, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_DOCUMENTATION_URI,
                    Mutability = Mutability.readOnly
                };
                documentationUriScheme.AddReferenceTypes("external");

                return documentationUriScheme;
            }
        }

        public static AttributeScheme PatchAttributeScheme
        {
            get
            {
                var patchScheme = new AttributeScheme("patch", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_PATCH,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                patchScheme.AddSubAttribute(SupportedSubAttributeScheme);

                return patchScheme;
            }
        }

        public static AttributeScheme BulkAttributeScheme
        {
            get
            {
                var bulkScheme = new AttributeScheme("bulk", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                bulkScheme.AddSubAttribute(SupportedSubAttributeScheme);
                bulkScheme.AddSubAttribute(MaxOperationsSubAttributeScheme);
                bulkScheme.AddSubAttribute(MaxPayloadSizeSubAttributeScheme);

                return bulkScheme;
            }
        }

        public static AttributeScheme EtagAttributeScheme
        {
            get
            {
                var etagScheme = new AttributeScheme("etag", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_ETAG,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                etagScheme.AddSubAttribute(SupportedSubAttributeScheme);

                return etagScheme;
            }
        }

        public static AttributeScheme FilterAttributeScheme
        {
            get
            {
                var filterScheme = new AttributeScheme("filter", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_FILTER,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                filterScheme.AddSubAttribute(SupportedSubAttributeScheme);
                filterScheme.AddSubAttribute(MaxResultsSubAttributeScheme);

                return filterScheme;
            }
        }

        public static AttributeScheme ChangePasswordAttributeScheme
        {
            get
            {
                var changePasswordScheme = new AttributeScheme("changePassword", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_CHANGE_PASSWORD,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                changePasswordScheme.AddSubAttribute(SupportedSubAttributeScheme);

                return changePasswordScheme;
            }
        }

        public static AttributeScheme SortAttributeScheme
        {
            get
            {
                var sortScheme = new AttributeScheme("sort", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_SORT,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                sortScheme.AddSubAttribute(SupportedSubAttributeScheme);

                return sortScheme;
            }
        }

        public static AttributeScheme AuthenticationSchemesAttributeScheme
        {
            get
            {
                var authenticationSchemesScheme = new AttributeScheme("authenticationSchemes", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
                authenticationSchemesScheme.AddSubAttribute(SampleMultivaluedAttributes.TypeAuthenticationSchemesAttributeScheme);
                authenticationSchemesScheme.AddSubAttribute(NameSubAttributeScheme);
                authenticationSchemesScheme.AddSubAttribute(DescriptionSubAttributeScheme);
                authenticationSchemesScheme.AddSubAttribute(SpecUriSubAttributeScheme);
                authenticationSchemesScheme.AddSubAttribute(DocumentationUriSubAttributeScheme);

                return authenticationSchemesScheme;
            }
        }

        public static AttributeScheme SupportedSubAttributeScheme
        {
            get
            {
                return new("supported", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_SUPPORTED,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme MaxOperationsSubAttributeScheme
        {
            get
            {
                return new("maxOperations", AttributeDataType.integer, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK_MAX_OPERATIONS,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme MaxPayloadSizeSubAttributeScheme
        {
            get
            {
                return new("maxPayloadSize", AttributeDataType.integer, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_BULK_MAX_PAYLOAD_SIZE,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme MaxResultsSubAttributeScheme
        {
            get
            {
                return new("maxResults", AttributeDataType.integer, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_CONFIG_FILTER_MAX_RESULTS,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme NameSubAttributeScheme
        {
            get
            {
                return new("name", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_NAME,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme DescriptionSubAttributeScheme
        {
            get
            {
                return new("description", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_DESCRIPTION,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme SpecUriSubAttributeScheme
        {
            get
            {
                var specUriScheme = new AttributeScheme("specUri", AttributeDataType.reference, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_SPEC_URI,
                    Mutability = Mutability.readOnly,
                };
                specUriScheme.AddReferenceTypes("external");

                return specUriScheme;
            }
        }

        public static AttributeScheme DocumentationUriSubAttributeScheme
        {
            get
            {
                var documentationUriScheme = new AttributeScheme("documentationUri", AttributeDataType.reference, false)
                {
                    Description = SampleConstants.DESCRIPTION_SERVICE_PROVIDER_AUTHENTICATION_SCHEMES_DOCUMENTATION_URI,
                    Mutability = Mutability.readOnly,
                };
                documentationUriScheme.AddReferenceTypes("external");

                return documentationUriScheme;
            }
        }
    }
}
