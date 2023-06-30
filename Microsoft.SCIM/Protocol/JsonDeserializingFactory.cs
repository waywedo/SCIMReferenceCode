//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public delegate Resource JsonDeserializingFactory(IReadOnlyDictionary<string, object> json);
}