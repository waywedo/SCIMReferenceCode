//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class PatchRequest2 : PatchRequest2Base<PatchOperation2Combined>
    {
        public PatchRequest2()
        {
        }

        public PatchRequest2(IReadOnlyCollection<PatchOperation2Combined> operations) : base(operations)
        {
        }
    }
}
