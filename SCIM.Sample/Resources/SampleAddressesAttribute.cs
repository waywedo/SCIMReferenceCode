// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleAddressesAttribute
    {
        public static AttributeScheme FormattedAddressAttributeScheme
        {
            get
            {
                return new("formatted", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_FORMATTED_ADDRESS
                };
            }
        }

        public static AttributeScheme StreetAddressAttributeScheme
        {
            get
            {
                return new("streetAddress", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_STREET_ADDRESS
                };
            }
        }

        public static AttributeScheme CountryAddressAttributeScheme
        {
            get
            {
                return new("country", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ADDRESS_COUNTRY
                };
            }
        }

        public static AttributeScheme LocalityAddressAttributeScheme
        {
            get
            {
                return new("locality", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ADDRESS_LOCALITY
                };
            }
        }

        public static AttributeScheme PostalCodeAddressAttributeScheme
        {
            get
            {
                return new("postalCode", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ADDRESS_POSTAL_CODE
                };
            }
        }

        public static AttributeScheme RegionAddressAttributeScheme
        {
            get
            {
                return new("region", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ADDRESS_REGION
                };
            }
        }
    }
}
