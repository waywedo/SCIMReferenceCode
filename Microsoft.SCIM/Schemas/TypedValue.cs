//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class TypedValue : TypedItem
    {
        [DataMember(Name = AttributeNames.VALUE, Order = 0)]
        public string Value { get; set; }
    }
}