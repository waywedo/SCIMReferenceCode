// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleGroupAttributes
    {
        public static AttributeScheme GroupDisplayNameAttributeScheme
        {
            get
            {
                return new("displayName", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_GROUP_DISPLAY_NAME,
                    Required = true,
                    Uniqueness = Uniqueness.server
                };
            }
        }

        public static AttributeScheme MembersAttributeScheme
        {
            get
            {
                var membersScheme = new AttributeScheme("members", AttributeDataType.complex, true)
                {
                    Description = SampleConstants.DESCRIPTION_MEMBERS
                };
                membersScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);
                membersScheme.AddSubAttribute(SampleMultivaluedAttributes.TypeSubAttributeScheme);

                return membersScheme;
            }
        }
    }
}
