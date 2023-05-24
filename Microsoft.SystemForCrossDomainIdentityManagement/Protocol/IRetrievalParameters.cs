//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IRetrievalParameters
    {
        IReadOnlyCollection<string> ExcludedAttributePaths { get; }
        string Path { get; }
        IReadOnlyCollection<string> RequestedAttributePaths { get; }
        string SchemaIdentifier { get; }
    }
}