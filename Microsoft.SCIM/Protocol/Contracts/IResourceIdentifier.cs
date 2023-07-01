//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IResourceIdentifier
    {
        string Identifier { get; set; }
        string SchemaIdentifier { get; set; }
    }
}