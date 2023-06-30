//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class ExtensionAttributeEnterpriseUserBase
    {
        [DataMember(Name = AttributeNames.COST_CENTER, IsRequired = false, EmitDefaultValue = false)]
        public string CostCenter { get; set; }

        [DataMember(Name = AttributeNames.DEPARTMENT, IsRequired = false, EmitDefaultValue = false)]
        public string Department { get; set; }

        [DataMember(Name = AttributeNames.DIVISION, IsRequired = false, EmitDefaultValue = false)]
        public string Division { get; set; }

        [DataMember(Name = AttributeNames.EMPLOYEE_NUMBER, IsRequired = false, EmitDefaultValue = false)]
        public string EmployeeNumber { get; set; }

        [DataMember(Name = AttributeNames.ORGANIZATION, IsRequired = false, EmitDefaultValue = false)]
        public string Organization { get; set; }
    }
}
