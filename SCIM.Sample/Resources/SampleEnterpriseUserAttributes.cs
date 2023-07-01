// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;

namespace SCIM.Sample.Resources
{
    public static class SampleEnterpriseUserAttributes
    {
        public static AttributeScheme ManagerAttributeScheme
        {
            get
            {
                var managerScheme = new AttributeScheme("manager", AttributeDataType.complex, false)
                {
                    Description = SampleConstants.DESCRIPTION_MANAGER
                };
                managerScheme.AddSubAttribute(SampleMultivaluedAttributes.ValueSubAttributeScheme);

                return managerScheme;
            }
        }

        public static AttributeScheme EmployeeNumberAttributeScheme
        {
            get
            {
                return new("employeeNumber", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_EMPLOYEE_NUMBER
                };
            }
        }

        public static AttributeScheme CostcenterAttributeScheme
        {
            get
            {
                return new("costCenter", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_COST_CENTER
                };
            }
        }

        public static AttributeScheme OrganizationAttributeScheme
        {
            get
            {
                return new("organization", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_ORGANIZATION
                };
            }
        }

        public static AttributeScheme DivisionAttributeScheme
        {
            get
            {
                return new("division", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_DIVISION
                };
            }
        }

        public static AttributeScheme DepartmentAttributeScheme
        {
            get
            {
                return new("department", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DESCRIPTION_DEPARTMENT
                };
            }
        }
    }
}
