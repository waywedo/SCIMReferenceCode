//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas.Contracts
{
    public interface IResourceJsonDeserializingFactory<TOutput> : ISchematizedJsonDeserializingFactory<TOutput>
        where TOutput : Resource
    {
    }
}