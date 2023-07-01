//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class RoleBase : TypedItem
    {
        [DataMember(Name = AttributeNames.DISPLAY, IsRequired = false, EmitDefaultValue = false)]
        public string Display { get; set; }

        [DataMember(Name = AttributeNames.VALUE, IsRequired = false, EmitDefaultValue = false)]
        public string Value { get; set; }
    }
}