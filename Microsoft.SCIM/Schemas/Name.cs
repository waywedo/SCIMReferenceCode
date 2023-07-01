//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class Name
    {
        [DataMember(Name = AttributeNames.FORMATTED, Order = 0, IsRequired = false, EmitDefaultValue = false)]
        public string Formatted { get; set; }

        [DataMember(Name = AttributeNames.FAMILY_NAME, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string FamilyName { get; set; }

        [DataMember(Name = AttributeNames.GIVEN_NAME, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string GivenName { get; set; }

        [DataMember(Name = AttributeNames.HONORIFIC_PREFIX, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string HonorificPrefix { get; set; }

        [DataMember(Name = AttributeNames.HONORIFIC_SUFFIX, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string HonorificSuffix { get; set; }
    }
}