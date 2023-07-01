// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleUserAttributes
    {
        public static AttributeScheme UserNameAttributeScheme
        {
            get
            {
                return new("userName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_USER_NAME,
                    Required = true,
                    Uniqueness = Uniqueness.server
                };
            }
        }

        public static AttributeScheme NameAttributeScheme
        {
            get
            {
                var nameScheme = new AttributeScheme("name", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_NAME
                };
                nameScheme.AddSubAttribute(SampleNameAttribute.FormattedNameAttributeScheme);
                nameScheme.AddSubAttribute(SampleNameAttribute.GivenNameAttributeScheme);
                nameScheme.AddSubAttribute(SampleNameAttribute.FamilyNameAttributeScheme);
                nameScheme.AddSubAttribute(SampleNameAttribute.HonorificPrefixAttributeScheme);
                nameScheme.AddSubAttribute(SampleNameAttribute.HonorificSuffixAttributeScheme);

                return nameScheme;
            }
        }

        public static AttributeScheme DisplayNameAttributeScheme
        {
            get
            {
                return new("displayName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_DISPLAY_NAME
                };
            }
        }

        public static AttributeScheme NickNameAttributeScheme
        {
            get
            {
                return new("nickName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_NICK_NAME
                };
            }
        }

        public static AttributeScheme TitleAttributeScheme
        {
            get
            {
                return new("title", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TITLE
                };
            }
        }

        public static AttributeScheme UserTypeAttributeScheme
        {
            get
            {
                return new("userType", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_USER_TYPE
                };
            }
        }

        public static AttributeScheme PreferredLanguageAttrbiuteScheme
        {
            get
            {
                return new("preferredLanguage", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_PREFERRED_LANGUAGE
                };
            }
        }

        public static AttributeScheme LocaleAttributeScheme
        {
            get
            {
                return new("locale", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_LOCALE
                };
            }
        }

        public static AttributeScheme TimezoneAttributeScheme
        {
            get
            {
                return new("timezone", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TIME_ZONE
                };
            }
        }

        public static AttributeScheme ActiveAttributeScheme
        {
            get
            {
                return new("active", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_ACTIVE
                };
            }
        }

        public static AttributeScheme EmailsAttributeScheme
        {
            get
            {
                var emailsScheme = new AttributeScheme("emails", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_EMAILS
                };
                emailsScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);
                emailsScheme.AddSubAttribute(SampleMultivaluedAttributes.Type2SubAttributeScheme);
                emailsScheme.AddSubAttribute(SampleMultivaluedAttributes.PrimarySubAttributeScheme);

                return emailsScheme;
            }
        }

        public static AttributeScheme PhoneNumbersAttributeScheme
        {
            get
            {
                var phoneNumbersScheme = new AttributeScheme("phoneNumbers", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_PHONE_NUMBERS
                };
                phoneNumbersScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);
                phoneNumbersScheme.AddSubAttribute(SampleMultivaluedAttributes.Type2SubAttributeScheme);
                phoneNumbersScheme.AddSubAttribute(SampleMultivaluedAttributes.PrimarySubAttributeScheme);

                return phoneNumbersScheme;
            }
        }

        public static AttributeScheme AddressesAttributeScheme
        {
            get
            {
                var addressesScheme = new AttributeScheme("addresses", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_ADDRESSES
                };
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.FormattedAddressAttributeScheme);
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.StreetAddressAttributeScheme);
                addressesScheme.AddSubAttribute(SampleMultivaluedAttributes.Type2SubAttributeScheme);
                addressesScheme.AddSubAttribute(SampleMultivaluedAttributes.PrimarySubAttributeScheme);
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.CountryAddressAttributeScheme);
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.LocalityAddressAttributeScheme);
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.PostalCodeAddressAttributeScheme);
                addressesScheme.AddSubAttribute(SampleAddressesAttribute.RegionAddressAttributeScheme);

                return addressesScheme;
            }
        }

        public static AttributeScheme ImsAttributeScheme
        {
            get
            {
                var imsScheme = new AttributeScheme("ims", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_IMS
                };
                imsScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);
                imsScheme.AddSubAttribute(SampleMultivaluedAttributes.TypeImsSubAttributeScheme);
                imsScheme.AddSubAttribute(SampleMultivaluedAttributes.PrimarySubAttributeScheme);

                return imsScheme;
            }
        }

        public static AttributeScheme RolessAttributeScheme
        {
            get
            {
                var rolesScheme = new AttributeScheme("roles", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_ROLES
                };
                rolesScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);
                rolesScheme.AddSubAttribute(SampleMultivaluedAttributes.DisplaySubAttributeScheme);
                rolesScheme.AddSubAttribute(SampleMultivaluedAttributes.TypeDefaultSubAttributeScheme);
                rolesScheme.AddSubAttribute(SampleMultivaluedAttributes.PrimarySubAttributeScheme);

                return rolesScheme;
            }
        }
    }
}
