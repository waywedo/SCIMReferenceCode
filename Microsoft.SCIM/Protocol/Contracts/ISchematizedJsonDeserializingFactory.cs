//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol.Contracts
{
    internal interface ISchematizedJsonDeserializingFactory : IGroupDeserializer,
        IPatchRequest2Deserializer, IUserDeserializer
    {
    }
}