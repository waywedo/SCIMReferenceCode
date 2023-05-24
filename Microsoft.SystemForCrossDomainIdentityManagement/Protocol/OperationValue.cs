//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class OperationValue
    {
        private const string TEMPLATE = "{0} {1}";

        [DataMember(Name = ProtocolAttributeNames.REFERENCE, Order = 0, IsRequired = false, EmitDefaultValue = false)]
        public string Reference { get; set; }

        [DataMember(Name = AttributeNames.VALUE, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, Value, Reference).Trim();
        }
    }
}