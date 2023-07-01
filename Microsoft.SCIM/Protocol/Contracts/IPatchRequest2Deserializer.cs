//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IPatchRequest2Deserializer
    {
        ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior { get; set; }
    }
}
