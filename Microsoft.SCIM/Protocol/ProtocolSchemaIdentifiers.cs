//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol
{
    public static class ProtocolSchemaIdentifiers
    {
        private const string ERROR = "Error";
        private const string OPERATION_PATCH = "PatchOp";
        private const string VERSION_MESSAGES_2 = "2.0:";
        private const string PREFIX_MESSAGES = "urn:ietf:params:scim:api:messages:";
        private const string RESPONSE_LIST = "ListResponse";
        private const string REQUEST_BULK = "BulkRequest";
        private const string RESPONSE_BULK = "BulkResponse";

        public const string PREFIX_MESSAGES_2 = PREFIX_MESSAGES + VERSION_MESSAGES_2;

        public const string VERSION_2_ERROR = PREFIX_MESSAGES_2 + ERROR;
        public const string VERSION_2_LIST_RESPONSE = PREFIX_MESSAGES_2 + RESPONSE_LIST;
        public const string VERSION_2_PATCH_OPERATION = PREFIX_MESSAGES_2 + OPERATION_PATCH;
        public const string VERSION_2_BULK_REQUEST = PREFIX_MESSAGES_2 + REQUEST_BULK;
        public const string VERSION_2_BULK_RESPONSE = PREFIX_MESSAGES_2 + RESPONSE_BULK;
    }
}
