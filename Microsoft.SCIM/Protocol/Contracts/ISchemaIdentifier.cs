//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface ISchemaIdentifier
    {
        string Value { get; }

        string FindPath();
        bool TryFindPath(out string path);
    }
}