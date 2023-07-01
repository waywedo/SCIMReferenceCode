//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IResourceRetrievalParameters : IRetrievalParameters
    {
        IResourceIdentifier ResourceIdentifier { get; }
    }
}