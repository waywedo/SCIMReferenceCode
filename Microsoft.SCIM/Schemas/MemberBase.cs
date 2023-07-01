//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class MemberBase
    {
        internal MemberBase()
        {
        }

        [DataMember(Name = AttributeNames.TYPE, IsRequired = false, EmitDefaultValue = false)]
        public string TypeName { get; set; }

        [DataMember(Name = AttributeNames.VALUE)]
        public string Value { get; set; }
    }
}