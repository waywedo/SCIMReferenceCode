//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class ExtensionAttributeWindowsAzureActiveDirectoryGroup
    {
        [DataMember(Name = AttributeNames.ELECTRONIC_MAIL_ADDRESSES)]
        public IEnumerable<ElectronicMailAddress> ElectronicMailAddresses { get; set; }

        [DataMember(Name = AttributeNames.EXTERNAL_IDENTIFIER)]
        public string ExternalIdentifier { get; set; }

        [DataMember(Name = AttributeNames.MAIL_ENABLED, IsRequired = false)]
        public bool MailEnabled { get; set; }

        [DataMember(Name = AttributeNames.SECURITY_ENABLED, IsRequired = false)]
        public bool SecurityEnabled { get; set; }
    }
}