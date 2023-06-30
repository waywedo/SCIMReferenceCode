//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class AddressBase : TypedItem
    {
        public const string HOME = "home";
        public const string OTHER = "other";
        public const string UNTYPED = "untyped";
        public const string WORK = "work";

        internal AddressBase()
        {
        }

        [DataMember(Name = AttributeNames.COUNTRY, IsRequired = false, EmitDefaultValue = false)]
        public string Country { get; set; }

        [DataMember(Name = AttributeNames.FORMATTED, IsRequired = false, EmitDefaultValue = false)]
        public string Formatted { get; set; }

        [DataMember(Name = AttributeNames.LOCALITY, IsRequired = false, EmitDefaultValue = false)]
        public string Locality { get; set; }

        [DataMember(Name = AttributeNames.POSTAL_CODE, IsRequired = false, EmitDefaultValue = false)]
        public string PostalCode { get; set; }

        [DataMember(Name = AttributeNames.REGION, IsRequired = false, EmitDefaultValue = false)]
        public string Region { get; set; }

        [DataMember(Name = AttributeNames.STREET_ADDRESS, IsRequired = false, EmitDefaultValue = false)]
        public string StreetAddress { get; set; }
    }
}