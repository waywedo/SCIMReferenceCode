//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;

namespace Microsoft.SCIM
{
    public class SchemaIdentifier : ISchemaIdentifier
    {
        public SchemaIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        public string FindPath()
        {
            if (!TryFindPath(out string result))
            {
                throw new NotSupportedException(Value);
            }

            return result;
        }

        public bool TryFindPath(out string path)
        {
            path = null;

            switch (Value)
            {
                case SchemaIdentifiers.CORE_2_ENTERPRISE_USER:
                case SchemaIdentifiers.CORE_2_USER:
                    path = ProtocolConstants.PATH_USERS;
                    return true;
                case SchemaIdentifiers.CORE_2_GROUP:
                    path = ProtocolConstants.PATH_GROUPS;
                    return true;
                case SchemaIdentifiers.NONE:
                    path = SchemaConstants.PATH_INTERFACE;
                    return true;
                default:
                    return false;
            }
        }
    }
}