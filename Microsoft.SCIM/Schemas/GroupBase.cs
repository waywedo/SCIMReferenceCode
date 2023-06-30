//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class GroupBase : Resource
    {
        [DataMember(Name = AttributeNames.DISPLAY_NAME)]
        public virtual string DisplayName { get; set; }

        [DataMember(Name = AttributeNames.MEMBERS, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Member> Members { get; set; }
    }
}