//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class UserBase : Resource
    {
        [DataMember(Name = AttributeNames.USER_NAME)]
        public virtual string UserName { get; set; }
    }
}