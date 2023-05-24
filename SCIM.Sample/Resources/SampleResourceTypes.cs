// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM;

namespace SCIM.Sample.Resources
{
    using System;

    public class SampleResourceTypes
    {
        public static Core2ResourceType UserResourceType
        {
            get
            {
                Core2ResourceType userResource = new Core2ResourceType
                {
                    Identifier = Types.USER,
                    Endpoint = new Uri($"{SampleConstants.SampleScimEndpoint}/Users"),
                    Schema = SampleConstants.UserEnterpriseSchema
                };

                return userResource;
            }
        }

        public static Core2ResourceType GroupResourceType
        {
            get
            {
                Core2ResourceType groupResource = new Core2ResourceType
                {
                    Identifier = Types.GROUP,
                    Endpoint = new Uri($"{SampleConstants.SampleScimEndpoint}/Groups"),
                    Schema = $"{SampleConstants.Core2SchemaPrefix}{Types.GROUP}"
                };

                return groupResource;
            }
        }
    }
}
