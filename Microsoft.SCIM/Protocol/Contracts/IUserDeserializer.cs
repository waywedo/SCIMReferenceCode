//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IUserDeserializer
    {
        IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; set; }
    }
}
