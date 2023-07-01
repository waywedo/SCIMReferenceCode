// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleMultivaluedAttributes
    {
        public static AttributeScheme ValueSubAttributeScheme
        {
            get
            {
                return new("value", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_VALUE,
                };
            }
        }

        public static AttributeScheme TypeSubAttributeScheme
        {
            get
            {
                var typeScheme = new AttributeScheme("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TYPE,
                    Mutability = Mutability.immutable
                };
                typeScheme.AddCanonicalValues(Types.GROUP);
                typeScheme.AddCanonicalValues(Types.USER);

                return typeScheme;
            }
        }

        public static AttributeScheme DisplaySubAttributeScheme
        {
            get
            {
                return new("display", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_DISPLAY
                };
            }
        }

        public static AttributeScheme Type2SubAttributeScheme
        {
            get
            {
                var typeScheme = new AttributeScheme("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TYPE
                };
                typeScheme.AddCanonicalValues("work");
                typeScheme.AddCanonicalValues("home");
                typeScheme.AddCanonicalValues("other");

                return typeScheme;
            }
        }

        public static AttributeScheme TypeAuthenticationSchemesAttributeScheme
        {
            get
            {
                var typeScheme = new AttributeScheme("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TYPE,
                    Required = true
                };
                typeScheme.AddCanonicalValues("oauth");
                typeScheme.AddCanonicalValues("oauth2");
                typeScheme.AddCanonicalValues("oauthbearertoken");
                typeScheme.AddCanonicalValues("httpbasic");
                typeScheme.AddCanonicalValues("httpdigest");

                return typeScheme;
            }
        }

        public static AttributeScheme TypeImsSubAttributeScheme
        {
            get
            {
                var typeScheme = new AttributeScheme("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TYPE
                };
                typeScheme.AddCanonicalValues("aim");
                typeScheme.AddCanonicalValues("gtalk");
                typeScheme.AddCanonicalValues("icq");
                typeScheme.AddCanonicalValues("xmpp");
                typeScheme.AddCanonicalValues("msn");
                typeScheme.AddCanonicalValues("skype");
                typeScheme.AddCanonicalValues("qq");
                typeScheme.AddCanonicalValues("yahoo");

                return typeScheme;
            }
        }

        public static AttributeScheme TypeDefaultSubAttributeScheme
        {
            get
            {
                return new("type", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_TYPE,
                };
            }
        }

        public static AttributeScheme PrimarySubAttributeScheme
        {
            get
            {
                return new("primary", AttributeDataType.boolean, false)
                {
                    Description = SampleConstants.DESCRIPTION_PRIMARY
                };
            }
        }
    }
}
