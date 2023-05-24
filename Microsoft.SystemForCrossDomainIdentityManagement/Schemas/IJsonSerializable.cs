//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IJsonSerializable
    {
        Dictionary<string, object> ToJson();
        string Serialize();
    }
}
