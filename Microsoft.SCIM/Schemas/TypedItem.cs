//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class TypedItem
    {
        [DataMember(Name = AttributeNames.TYPE)]
        public string ItemType { get; set; }

        [DataMember(Name = AttributeNames.PRIMARY, IsRequired = false)]
        public bool Primary { get; set; }
    }
}