//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    [DataContract]
    public abstract class PatchRequestBase : Schematized
    {
    }
}