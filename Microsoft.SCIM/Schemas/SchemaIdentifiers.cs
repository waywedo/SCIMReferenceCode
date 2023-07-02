//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas
{
    public static class SchemaIdentifiers
    {
        public const string EXTENSION = "extension:";

        public const string EXTENSION_ENTERPRISE_2 = EXTENSION + "enterprise:2.0:";

        public const string NONE = "/";

        public const string PREFIX_TYPES_1 = "urn:scim:schemas:";
        public const string PREFIX_TYPES_2 = "urn:ietf:params:scim:schemas:";

        public const string VERSION_SCHEMAS_CORE_2 = "core:2.0:";

        public const string CORE_2_ENTERPRISE_USER = PREFIX_TYPES_2 + EXTENSION_ENTERPRISE_2 + Types.USER;
        public const string CORE_2_GROUP = PREFIX_TYPES_2 + VERSION_SCHEMAS_CORE_2 + Types.GROUP;
        public const string CORE_2_RESOURCE_TYPE = PREFIX_TYPES_2 + EXTENSION_ENTERPRISE_2 + Types.RESOURCE_TYPE;
        public const string CORE_2_SERVICE_CONFIGURATION = PREFIX_TYPES_2 + VERSION_SCHEMAS_CORE_2 + Types.SERVICE_PROVIDER_CONFIGURATION;
        public const string CORE_2_USER = PREFIX_TYPES_2 + VERSION_SCHEMAS_CORE_2 + Types.USER;
        public const string CORE_2_SCHEMA = PREFIX_TYPES_2 + VERSION_SCHEMAS_CORE_2 + Types.SCHEMA;
        public const string PREFIX_EXTENSION = PREFIX_TYPES_2 + EXTENSION;
    }
}