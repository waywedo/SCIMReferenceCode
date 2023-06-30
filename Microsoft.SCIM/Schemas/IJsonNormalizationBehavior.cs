//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IJsonNormalizationBehavior
    {
        IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json);
    }
}