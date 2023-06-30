// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM
{
    public static class ServiceConstants
    {
        public const string PATH_SEGMENT_RESOURCE_TYPES = "ResourceTypes";
        public const string PATH_SEGMENT_SCHEMAS = "Schemas";
        public const string PATH_SEGMENT_SERVICE_PROVIDER_CONFIGURATION = "ServiceProviderConfig";
        public const string ROUTE_GROUPS = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + ProtocolConstants.PATH_GROUPS;
        public const string ROUTE_RESOURCE_TYPES = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + PATH_SEGMENT_RESOURCE_TYPES;
        public const string ROUTE_SCHEMAS = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + PATH_SEGMENT_SCHEMAS;
        public const string ROUTE_SERVICE_CONFIGURATION = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + PATH_SEGMENT_SERVICE_PROVIDER_CONFIGURATION;
        public const string ROUTE_USERS = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + ProtocolConstants.PATH_USERS;
        public const string ROUTE_BULK = SchemaConstants.PATH_INTERFACE + SEPARATOR_SEGMENTS + ProtocolConstants.PATH_BULK;
        public const string SEPARATOR_SEGMENTS = "/";
    }
}
