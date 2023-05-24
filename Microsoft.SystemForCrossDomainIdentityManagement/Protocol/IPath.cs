//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IPath
    {
        string AttributePath { get; }
        string SchemaIdentifier { get; }
        IReadOnlyCollection<IFilter> SubAttributes { get; }
        IPath ValuePath { get; }
    }
}