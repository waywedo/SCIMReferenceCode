//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    public delegate Resource JsonDeserializingFactory(IReadOnlyDictionary<string, object> json);
}