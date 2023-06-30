// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class ExtensionAttributeEnterpriseUser2 : ExtensionAttributeEnterpriseUserBase
    {
        [DataMember(Name = AttributeNames.MANAGER, IsRequired = false, EmitDefaultValue = false)]
        public Manager Manager { get; set; }
    }
}