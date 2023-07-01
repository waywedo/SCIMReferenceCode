//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class Core2EnterpriseUserBase : Core2UserBase
    {
        protected Core2EnterpriseUserBase()
        {
            AddSchema(SchemaIdentifiers.CORE_2_ENTERPRISE_USER);
            EnterpriseExtension = new ExtensionAttributeEnterpriseUser2();
        }

        [DataMember(Name = AttributeNames.EXTENSION_ENTERPRISE_USER_2)]
        public ExtensionAttributeEnterpriseUser2 EnterpriseExtension { get; set; }
    }
}