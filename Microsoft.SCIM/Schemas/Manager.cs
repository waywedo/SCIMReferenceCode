//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class Manager
    {
        [DataMember(Name = AttributeNames.VALUE)]
        public string Value { get; set; }
    }
}