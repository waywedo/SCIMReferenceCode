// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleSchemaAttributes
    {
        public static AttributeScheme NameAttributeScheme
        {
            get
            {
                return new("name", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_SCHEMA_NAME,
                    Mutability = Mutability.readOnly,
                    Required = true
                };
            }
        }

        public static AttributeScheme DescriptionAttributeScheme
        {
            get
            {
                return new("description", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_SCHEMA_NAME,
                    Mutability = Mutability.readOnly,
                };
            }
        }

        public static AttributeScheme AttributesAttributeScheme
        {
            get
            {
                var attributesScheme = new AttributeScheme("attributes", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_SCHEMA_ATTRIBUTES,
                    Mutability = Mutability.readOnly,
                };
                attributesScheme.AddSubAttribute(NameSubAttributeScheme);
                attributesScheme.AddSubAttribute(TypeSubAttributeScheme);
                attributesScheme.AddSubAttribute(MultiValuedSubAttributeScheme);
                attributesScheme.AddSubAttribute(DescriptionSubAttributeScheme);
                attributesScheme.AddSubAttribute(RequiredSubAttributeScheme);
                attributesScheme.AddSubAttribute(CanonicalValuesSubAttributeScheme);
                attributesScheme.AddSubAttribute(CaseExactSubAttributeScheme);
                attributesScheme.AddSubAttribute(MutabilitySubAttributeScheme);
                attributesScheme.AddSubAttribute(ReturnedSubAttributeScheme);
                attributesScheme.AddSubAttribute(UniquenessSubAttributeScheme);
                attributesScheme.AddSubAttribute(ReferenceTypesSubAttributeScheme);
                attributesScheme.AddSubAttribute(SubAttributesSubAttributeScheme);

                return attributesScheme;
            }
        }

        public static AttributeScheme NameSubAttributeScheme
        {
            get
            {
                return new("name", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_NAME,
                    Mutability = Mutability.readOnly,
                    Required = true,
                    CaseExact = true
                };
            }
        }

        public static AttributeScheme TypeSubAttributeScheme
        {
            get
            {
                var typeScheme = new AttributeScheme("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_TYPE,
                    Mutability = Mutability.readOnly,
                    Required = true,
                };
                typeScheme.AddCanonicalValues("string");
                typeScheme.AddCanonicalValues("complex");
                typeScheme.AddCanonicalValues("boolean");
                typeScheme.AddCanonicalValues("decimal");
                typeScheme.AddCanonicalValues("integer");
                typeScheme.AddCanonicalValues("dateTime");
                typeScheme.AddCanonicalValues("reference");

                return typeScheme;
            }
        }
        public static AttributeScheme MultiValuedSubAttributeScheme
        {
            get
            {
                return new("multiValued", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_MULTI_VALUED,
                    Mutability = Mutability.readOnly,
                    Required = true,
                };
            }
        }

        public static AttributeScheme DescriptionSubAttributeScheme
        {
            get
            {
                return new("description", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_DESCRIPTION,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
            }
        }

        public static AttributeScheme RequiredSubAttributeScheme
        {
            get
            {
                return new("required", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_REQUIRED,
                    Mutability = Mutability.readOnly,
                };
            }
        }

        public static AttributeScheme CanonicalValuesSubAttributeScheme
        {
            get
            {
                return new("canonicalValues", AttributeDataType.@string, true)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_CANONICAL_VALUES,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
            }
        }

        public static AttributeScheme CaseExactSubAttributeScheme
        {
            get
            {
                return new("caseExact", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_CASE_EXACT,
                    Mutability = Mutability.readOnly,
                };
            }
        }

        public static AttributeScheme MutabilitySubAttributeScheme
        {
            get
            {
                var mutabilityScheme = new AttributeScheme("mutability", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_MUTABILITY,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
                mutabilityScheme.AddCanonicalValues("readOnly");
                mutabilityScheme.AddCanonicalValues("readWrite");
                mutabilityScheme.AddCanonicalValues("immutable");
                mutabilityScheme.AddCanonicalValues("writeOnly");

                return mutabilityScheme;
            }
        }

        public static AttributeScheme ReturnedSubAttributeScheme
        {
            get
            {
                var returnedScheme = new AttributeScheme("returned", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_RETURNED,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
                returnedScheme.AddCanonicalValues("always");
                returnedScheme.AddCanonicalValues("never");
                returnedScheme.AddCanonicalValues("default");
                returnedScheme.AddCanonicalValues("request");

                return returnedScheme;
            }
        }

        public static AttributeScheme UniquenessSubAttributeScheme
        {
            get
            {
                var uniquenessScheme = new AttributeScheme("uniqueness", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_UNIQUENESS,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
                uniquenessScheme.AddCanonicalValues("none");
                uniquenessScheme.AddCanonicalValues("server");
                uniquenessScheme.AddCanonicalValues("global");

                return uniquenessScheme;
            }
        }

        public static AttributeScheme ReferenceTypesSubAttributeScheme
        {
            get
            {
                return new("referenceTypes", AttributeDataType.@string, true)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_REFERENCE_TYPES,
                    Mutability = Mutability.readOnly,
                    CaseExact = true
                };
            }
        }

        public static AttributeScheme SubAttributesSubAttributeScheme
        {
            get
            {
                var subAttributesScheme = new AttributeScheme("subAttributes", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_ATTRIBUTE_SUB_ATTRIBUTES,
                    Mutability = Mutability.readOnly,
                };
                subAttributesScheme.AddSubAttribute(NameSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(TypeSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(MultiValuedSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(DescriptionSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(RequiredSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(CanonicalValuesSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(CaseExactSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(MutabilitySubAttributeScheme);
                subAttributesScheme.AddSubAttribute(ReturnedSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(UniquenessSubAttributeScheme);
                subAttributesScheme.AddSubAttribute(ReferenceTypesSubAttributeScheme);

                return subAttributesScheme;
            }
        }
    }
}
