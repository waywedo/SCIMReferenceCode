// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleResourceTypes
    {
        public static Core2ResourceType UserResourceType
        {
            get
            {
                return new()
                {
                    Identifier = Types.USER,
                    Endpoint = new Uri($"{SampleConstants.SAMPLE_SCIM_ENDPOINT}/Users"),
                    Schema = SampleConstants.USER_ENTERPRISE_SCHEMA
                };
            }
        }

        public static Core2ResourceType GroupResourceType
        {
            get
            {
                return new()
                {
                    Identifier = Types.GROUP,
                    Endpoint = new Uri($"{SampleConstants.SAMPLE_SCIM_ENDPOINT}/Groups"),
                    Schema = $"{SampleConstants.CORE_2_SCHEMA_PREFIX}{Types.GROUP}"
                };
            }
        }
    }
}
