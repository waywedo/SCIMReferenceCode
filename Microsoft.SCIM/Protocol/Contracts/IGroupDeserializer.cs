//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IGroupDeserializer
    {
        IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }
    }
}
