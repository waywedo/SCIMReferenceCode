﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class BulkResponse2 : BulkOperations<BulkResponseOperation>
    {
        public BulkResponse2() : base(ProtocolSchemaIdentifiers.VERSION_2_BULK_RESPONSE)
        {
        }
    }
}
