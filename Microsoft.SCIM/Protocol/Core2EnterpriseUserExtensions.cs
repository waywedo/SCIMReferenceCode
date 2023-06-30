//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Microsoft.SCIM
{
    public static class Core2EnterpriseUserExtensions
    {
        public static void Apply(this Core2EnterpriseUser user, PatchRequest2Base<PatchOperation2> patch)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (patch == null)
            {
                return;
            }

            if (patch.Operations?.Any() != true)
            {
                return;
            }

            foreach (PatchOperation2 operation in patch.Operations)
            {
                user.Apply(operation);
            }
        }

        public static void Apply(this Core2EnterpriseUser user, PatchRequest2 patch)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));

            if (patch == null)
            {
                return;
            }

            if (patch.Operations?.Any() != true)
            {
                return;
            }

            foreach (PatchOperation2Combined operation in patch.Operations)
            {
                var operationInternal = new PatchOperation2
                {
                    OperationName = operation.OperationName,
                    Path = operation.Path
                };

                var values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);

                if (values == null)
                {
                    var value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value);
                    var valueSingle = new OperationValue { Value = value };

                    operationInternal.AddValue(valueSingle);
                }
                else
                {
                    foreach (var value in values)
                    {
                        operationInternal.AddValue(value);
                    }
                }

                user.Apply(operationInternal);
            }
        }

        private static void Apply(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation == null)
            {
                return;
            }

            if (operation.Path == null || string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(operation.Path.SchemaIdentifier)
                && (operation?.Path?.SchemaIdentifier?.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.OrdinalIgnoreCase) == true))
            {
                user.PatchEnterpriseExtension(operation);
                return;
            }

            OperationValue value;

            switch (operation.Path.AttributePath)
            {
                case AttributeNames.ACTIVE:
                    if (operation.Name != OperationName.Remove)
                    {
                        value = operation.Value.SingleOrDefault();

                        if (value != null && !string.IsNullOrWhiteSpace(value.Value) && bool.TryParse(value.Value, out bool active))
                        {
                            user.Active = active;
                        }
                    }
                    break;

                case AttributeNames.ADDRESSES:
                    user.PatchAddresses(operation);
                    break;

                case AttributeNames.DISPLAY_NAME:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if ((value == null) || string.Equals(user.DisplayName, value.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (value == null)
                    {
                        user.DisplayName = null;
                    }
                    else
                    {
                        user.DisplayName = value.Value;
                    }
                    break;

                case AttributeNames.ELECTRONIC_MAIL_ADDRESSES:
                    user.PatchElectronicMailAddresses(operation);
                    break;

                case AttributeNames.EXTERNAL_IDENTIFIER:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if ((value == null) || string.Equals(user.ExternalIdentifier, value.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (value == null)
                    {
                        user.ExternalIdentifier = null;
                    }
                    else
                    {
                        user.ExternalIdentifier = value.Value;
                    }
                    break;

                case AttributeNames.NAME:
                    user.PatchName(operation);
                    break;

                case AttributeNames.PHONE_NUMBERS:
                    user.PatchPhoneNumbers(operation);
                    break;

                case AttributeNames.PREFERRED_LANGUAGE:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if ((value == null) || string.Equals(user.PreferredLanguage, value.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (value == null)
                    {
                        user.PreferredLanguage = null;
                    }
                    else
                    {
                        user.PreferredLanguage = value.Value;
                    }
                    break;

                case AttributeNames.ROLES:
                    user.PatchRoles(operation);
                    break;

                case AttributeNames.TITLE:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if ((value == null) || string.Equals(user.Title, value.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (value == null)
                    {
                        user.Title = null;
                    }
                    else
                    {
                        user.Title = value.Value;
                    }
                    break;

                case AttributeNames.USER_NAME:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if ((value == null) || string.Equals(user.UserName, value.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (value == null)
                    {
                        user.UserName = null;
                    }
                    else
                    {
                        user.UserName = value.Value;
                    }
                    break;
            }
        }

        private static void PatchAddresses(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation == null)
            {
                return;
            }

            if (!string.Equals(AttributeNames.ADDRESSES, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (operation.Path.ValuePath == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                return;
            }

            var subAttribute = operation.Path.SubAttributes.SingleOrDefault();

            if (subAttribute == null)
            {
                return;
            }

            if ((operation.Value != null && operation.Value.Count != 1) || (operation.Value == null && operation.Name != OperationName.Remove))
            {
                return;
            }

            if (!string.Equals(AttributeNames.TYPE, subAttribute.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Address address;
            Address addressExisting;

            if (user.Addresses != null)
            {
                addressExisting = address = user.Addresses.SingleOrDefault(
                    (Address item) => string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)
                );
            }
            else
            {
                addressExisting = null;
                address = new Address { ItemType = subAttribute.ComparisonValue };
            }

            string value;

            if (string.Equals(AddressBase.WORK, subAttribute.ComparisonValue, StringComparison.Ordinal))
            {
                if (string.Equals(AttributeNames.COUNTRY, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.Country, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.Country = value;
                }

                if (string.Equals(AttributeNames.LOCALITY, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.Locality, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.Locality = value;
                }

                if (string.Equals(AttributeNames.POSTAL_CODE, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.PostalCode, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.PostalCode = value;
                }

                if (string.Equals(AttributeNames.REGION, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.Region, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.Region = value;
                }

                if (string.Equals(AttributeNames.STREET_ADDRESS, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.StreetAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.StreetAddress = value;
                }
            }

            if (string.Equals(AddressBase.OTHER, subAttribute.ComparisonValue, StringComparison.Ordinal))
            {
                if (string.Equals(AttributeNames.FORMATTED, operation.Path.ValuePath.AttributePath, StringComparison.Ordinal))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && OperationName.Remove == operation.Name && string.Equals(value, address.Formatted, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    address.Formatted = value;
                }
            }

            if (address == null || (string.IsNullOrWhiteSpace(address.Country) && string.IsNullOrWhiteSpace(address.Locality) && string.IsNullOrWhiteSpace(address.PostalCode)
                && string.IsNullOrWhiteSpace(address.Region) && string.IsNullOrWhiteSpace(address.StreetAddress) && string.IsNullOrWhiteSpace(address.Formatted)))
            {
                if (addressExisting != null)
                {
                    user.Addresses = user.Addresses.Where((item) => !string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)).ToArray();
                }

                return;
            }

            if (addressExisting != null)
            {
                return;
            }

            var addresses = new Address[] { address };

            if (user.Addresses == null)
            {
                user.Addresses = addresses;
            }
            else
            {
                user.Addresses = user.Addresses.Union(addresses).ToArray();
            }
        }

        private static void PatchCostCenter(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if ((value == null) || string.Equals(extension.CostCenter, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            extension.CostCenter = value?.Value;
        }

        private static void PatchDepartment(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if ((value == null) || string.Equals(extension.Department, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            extension.Department = value?.Value;
        }

        private static void PatchDivision(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if ((value == null) || string.Equals(extension.Division, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            extension.Division = value?.Value;
        }

        private static void PatchElectronicMailAddresses(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            user.ElectronicMailAddresses = ProtocolExtensions.PatchElectronicMailAddresses(user.ElectronicMailAddresses, operation);
        }

        private static void PatchEmployeeNumber(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if ((value == null) || string.Equals(extension.EmployeeNumber, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            extension.EmployeeNumber = value?.Value;
        }

        private static void PatchEnterpriseExtension(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation == null || operation.Path == null || string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                return;
            }

            var extension = user.EnterpriseExtension;

            switch (operation.Path.AttributePath)
            {
                case AttributeNames.COST_CENTER:
                    PatchCostCenter(extension, operation);
                    break;

                case AttributeNames.DEPARTMENT:
                    PatchDepartment(extension, operation);
                    break;

                case AttributeNames.DIVISION:
                    PatchDivision(extension, operation);
                    break;

                case AttributeNames.EMPLOYEE_NUMBER:
                    PatchEmployeeNumber(extension, operation);
                    break;

                case AttributeNames.MANAGER:
                    PatchManager(extension, operation);
                    break;

                case AttributeNames.ORGANIZATION:
                    PatchOrganization(extension, operation);
                    break;
            }
        }

        private static void PatchManager(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if (value == null || extension.Manager == null || string.Equals(extension.Manager.Value, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            if (value == null)
            {
                extension.Manager = null;
            }
            else
            {
                extension.Manager = new Manager
                {
                    Value = value.Value
                };
            }
        }

        private static void PatchName(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation == null || operation.Path == null)
            {
                return;
            }

            if (!string.Equals(AttributeNames.NAME, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (operation.Path.ValuePath == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                return;
            }

            if ((operation.Value != null && operation.Value.Count != 1) || (operation.Value == null && operation.Name != OperationName.Remove))
            {
                return;
            }

            Name nameExisting;
            var name = nameExisting = user.Name;

            name ??= new Name();

            string value;

            if (string.Equals(AttributeNames.GIVEN_NAME, operation.Path.ValuePath.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                value = operation.Value?.Single().Value;

                if (value != null && OperationName.Remove == operation.Name && string.Equals(value, name.GivenName, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }

                name.GivenName = value;
            }

            if (string.Equals(AttributeNames.FAMILY_NAME, operation.Path.ValuePath.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                value = operation.Value?.Single().Value;

                if (value != null && OperationName.Remove == operation.Name && string.Equals(value, name.FamilyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }

                name.FamilyName = value;
            }

            if (string.Equals(AttributeNames.FORMATTED, operation.Path.ValuePath.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                value = operation.Value?.Single().Value;

                if (value != null && OperationName.Remove == operation.Name && string.Equals(value, name.Formatted, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                name.Formatted = value;
            }

            if (string.IsNullOrWhiteSpace(name.FamilyName) && string.IsNullOrWhiteSpace(name.GivenName))
            {
                if (nameExisting != null)
                {
                    user.Name = null;
                }

                return;
            }

            if (nameExisting != null)
            {
                return;
            }

            user.Name = name;
        }

        private static void PatchOrganization(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            var value = operation.Value.SingleOrDefault();

            if (OperationName.Remove == operation.Name)
            {
                if ((value == null) || string.Equals(extension.Organization, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else
                {
                    return;
                }
            }

            extension.Organization = value?.Value;
        }

        private static void PatchPhoneNumbers(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation == null)
            {
                return;
            }

            if (!string.Equals(AttributeNames.PHONE_NUMBERS, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (operation.Path.ValuePath == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                return;
            }

            var subAttribute = operation.Path.SubAttributes.SingleOrDefault();

            if (subAttribute == null)
            {
                return;
            }

            if ((operation.Value != null && operation.Value.Count != 1) || (operation.Value == null && operation.Name != OperationName.Remove))
            {
                return;
            }

            if (!string.Equals(AttributeNames.TYPE, subAttribute.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var phoneNumberType = subAttribute.ComparisonValue;

            if (!string.Equals(phoneNumberType, PhoneNumberBase.FAX, StringComparison.Ordinal) && !string.Equals(phoneNumberType, PhoneNumberBase.MOBILE, StringComparison.Ordinal)
                && !string.Equals(phoneNumberType, PhoneNumberBase.WORK, StringComparison.Ordinal))
            {
                return;
            }

            PhoneNumber phoneNumber = null;
            PhoneNumber phoneNumberExisting = null;

            if (user.PhoneNumbers != null)
            {
                phoneNumberExisting = phoneNumber = user.PhoneNumbers.SingleOrDefault(
                    (PhoneNumber item) => string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)
                );
            }

            if (phoneNumber == null)
            {
                phoneNumberExisting = null;
                phoneNumber = new PhoneNumber() { ItemType = subAttribute.ComparisonValue };
            }

            var value = operation.Value?.Single().Value;

            if (value != null && OperationName.Remove == operation.Name && string.Equals(value, phoneNumber.Value, StringComparison.OrdinalIgnoreCase))
            {
                value = null;
            }

            phoneNumber.Value = value;

            if (string.IsNullOrWhiteSpace(phoneNumber.Value))
            {
                if (phoneNumberExisting != null)
                {
                    user.PhoneNumbers = user.PhoneNumbers.Where(
                        (PhoneNumber item) => !string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)
                    ).ToArray();
                }
                return;
            }

            if (phoneNumberExisting != null)
            {
                return;
            }

            var phoneNumbers = new PhoneNumber[] { phoneNumber };

            if (user.PhoneNumbers == null)
            {
                user.PhoneNumbers = phoneNumbers;
            }
            else
            {
                user.PhoneNumbers = user.PhoneNumbers.Union(phoneNumbers).ToArray();
            }
        }

        private static void PatchRoles(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            user.Roles = ProtocolExtensions.PatchRoles(user.Roles, operation);
        }
    }
}
