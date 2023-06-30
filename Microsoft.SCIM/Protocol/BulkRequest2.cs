//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{

    [DataContract]
    public sealed class BulkRequest2 : BulkOperations<BulkRequestOperation>
    {
        public BulkRequest2() : base(ProtocolSchemaIdentifiers.VERSION_2_BULK_REQUEST)
        {
        }

        [DataMember(Name = ProtocolAttributeNames.FAIL_ON_ERRORS, Order = 1)]
        public int? FailOnErrors { get; set; }
    }
}
