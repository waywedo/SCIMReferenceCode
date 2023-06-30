//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Microsoft.SCIM
{
    public static class ProtocolConstants
    {
        public const string CONTENT_TYPE = "application/scim+json";
        public const string PATH_GROUPS = "Groups";
        public const string PATH_USERS = "Users";
        public const string PATH_BULK = "Bulk";
        public const string PATH_WEB_BATCH_INTERFACE = SchemaConstants.PATH_INTERFACE + "/batch";

        public static readonly Lazy<JsonSerializerSettings> JsonSettings = new(() => InitializeSettings());

        private static JsonSerializerSettings InitializeSettings()
        {
            return new JsonSerializerSettings
            {
                Error = (object sender, ErrorEventArgs args) => args.ErrorContext.Handled = true
            };
        }
    }
}