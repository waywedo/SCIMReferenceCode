//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface ISchematizedJsonDeserializingFactory<TOutput> where TOutput : Schematized
    {
        TOutput Create(IReadOnlyDictionary<string, object> json);
    }
}