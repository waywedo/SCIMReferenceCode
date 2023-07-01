//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class Core2Metadata
    {
        [DataMember(Name = AttributeNames.RESOURCE_TYPE, Order = 0)]
        public string ResourceType { get; set; }

        [DataMember(Name = AttributeNames.CREATED, Order = 1)]
        public DateTime Created { get; set; }

        [DataMember(Name = AttributeNames.LAST_MODIFIED, Order = 2)]
        public DateTime LastModified { get; set; }

        [DataMember(Name = AttributeNames.VERSION, Order = 3)]
        public string Version { get; set; }

        [DataMember(Name = AttributeNames.LOCATION, Order = 4)]
        public string Location { get; set; }
    }
}