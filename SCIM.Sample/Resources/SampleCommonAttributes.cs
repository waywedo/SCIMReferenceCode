// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleCommonAttributes
    {
        public static AttributeScheme IdentiFierAttributeScheme
        {
            get
            {
                return new("id", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_IDENTIFIER
                };
            }
        }
    }
}
