// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleNameAttribute
    {
        public static AttributeScheme FormattedNameAttributeScheme
        {
            get
            {
                return new("formatted", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_FORMATTED_NAME
                };
            }
        }

        public static AttributeScheme GivenNameAttributeScheme
        {
            get
            {
                return new("givenName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_GIVEN_NAME
                };
            }
        }

        public static AttributeScheme FamilyNameAttributeScheme
        {
            get
            {
                return new("familyName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_FAMILY_NAME
                };
            }
        }

        public static AttributeScheme HonorificPrefixAttributeScheme
        {
            get
            {
                return new("honorificPrefix", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_HONORIFIC_PREFIX
                };
            }
        }

        public static AttributeScheme HonorificSuffixAttributeScheme
        {
            get
            {
                return new("honorificSuffix", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_HONORIFIC_SUFFIX
                };
            }
        }
    }
}
