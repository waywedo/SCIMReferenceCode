//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Newtonsoft.Json;

namespace Microsoft.SCIM.Protocol
{
    public static class ProtocolExtensions
    {
        private const string BULK_IDENTIFIER_PATTERN = $@"^((\s*)bulkId(\s*):(\s*)(?<{EXPRESSION_GROUP_NAME_BULK_IDENTIFIER}>[^\s]*))";
        private const string EXPRESSION_GROUP_NAME_BULK_IDENTIFIER = "identifier";

        public const string METHOD_NAME_DELETE = "DELETE";
        public const string METHOD_NAME_PATCH = "PATCH";

        private static readonly Lazy<HttpMethod> MethodPatch = new(() => new HttpMethod(METHOD_NAME_PATCH));
        private static readonly Lazy<Regex> BulkIdentifierExpression = new(() => new Regex(BULK_IDENTIFIER_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled));

        public static HttpMethod PatchMethod
        {
            get { return MethodPatch.Value; }
        }

        public static void Apply(this Core2Group group, PatchRequest2 patch)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

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
                var operationInternal = new PatchOperation2()
                {
                    OperationName = operation.OperationName,
                    Path = operation.Path
                };

                OperationValue[] values = null;

                if (operation?.Value != null)
                {
                    values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);
                }

                if (values == null)
                {
                    string value = null;

                    if (operation?.Value != null)
                    {
                        value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value);
                    }

                    var valueSingle = new OperationValue()
                    {
                        Value = value
                    };

                    operationInternal.AddValue(valueSingle);
                }
                else
                {
                    foreach (OperationValue value in values)
                    {
                        operationInternal.AddValue(value);
                    }
                }

                group.Apply(operationInternal);
            }
        }

        private static void Apply(this Core2Group group, PatchOperation2 operation)
        {
            if (operation == null || operation.Path == null || string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                return;
            }

            OperationValue value;

            switch (operation.Path.AttributePath)
            {
                case AttributeNames.DISPLAY_NAME:
                    value = operation.Value.SingleOrDefault();

                    if (OperationName.Remove == operation.Name)
                    {
                        if (value == null || string.Equals(group.DisplayName, value.Value, StringComparison.OrdinalIgnoreCase))
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
                        group.DisplayName = null;
                    }
                    else
                    {
                        group.DisplayName = value.Value;
                    }
                    break;

                case AttributeNames.MEMBERS:
                    if (operation.Value != null)
                    {
                        switch (operation.Name)
                        {
                            case OperationName.Add:
                                var membersToAdd = operation.Value.Select((item) => new Member() { Value = item.Value }).ToArray();
                                var buffer = new List<Member>();

                                group.Members ??= new List<Member>();

                                foreach (Member member in membersToAdd)
                                {
                                    //O(n) with the number of group members, so for large groups this is not optimal
                                    if (!group.Members.Any((item) =>
                                            string.Equals(item.Value, member.Value, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        buffer.Add(member);
                                    }
                                }

                                group.Members = group.Members.Concat(buffer.ToArray());
                                break;

                            case OperationName.Remove:
                                if (group.Members == null)
                                {
                                    break;
                                }

                                if (operation?.Value?.FirstOrDefault()?.Value == null)
                                {
                                    group.Members = Enumerable.Empty<Member>();
                                    break;
                                }

                                var members = new Dictionary<string, Member>(group.Members.Count());

                                foreach (Member item in group.Members)
                                {
                                    members.Add(item.Value, item);
                                }

                                foreach (OperationValue operationValue in operation.Value)
                                {
                                    if (members.TryGetValue(operationValue.Value, out Member removedMember))
                                    {
                                        members.Remove(operationValue.Value);
                                    }
                                }

                                group.Members = members.Values;
                                break;
                        }
                    }
                    break;
                case AttributeNames.EXTERNAL_IDENTIFIER:

                    value = operation.Value?.SingleOrDefault();

                    group.ExternalIdentifier = value?.Value;

                    break;
            }
        }

        private static Uri ComposeTypeIdentifier(Uri baseResourceIdentifier, string path)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var baseResourceIdentifierValue = baseResourceIdentifier.ToString();
            var resultValue = baseResourceIdentifierValue + SchemaConstants.PATH_INTERFACE + ServiceConstants.SEPARATOR_SEGMENTS + path;

            return new Uri(resultValue);
        }

        private static string GetPath(this Schematized schematized)
        {
            if (schematized.TryGetPath(out string path))
            {
                return path;
            }

            if (schematized.Is(SchemaIdentifiers.CORE_2_ENTERPRISE_USER))
            {
                return ProtocolConstants.PATH_USERS;
            }

            if (schematized.Is(SchemaIdentifiers.CORE_2_USER))
            {
                return ProtocolConstants.PATH_USERS;
            }

            if (schematized.Is(SchemaIdentifiers.CORE_2_GROUP))
            {
                return ProtocolConstants.PATH_GROUPS;
            }

            switch (schematized)
            {
                case UserBase:
                    return ProtocolConstants.PATH_USERS;
                case GroupBase:
                    return ProtocolConstants.PATH_GROUPS;
                default:
                    string unsupportedTypeName = schematized.GetType().FullName;
                    throw new NotSupportedException(unsupportedTypeName);
            }
        }

        public static Uri GetResourceIdentifier(this Resource resource, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resource.Identifier))
            {
                throw new InvalidOperationException(ProtocolResources.ExceptionInvalidResource);
            }

            if (resource.TryGetIdentifier(baseResourceIdentifier, out Uri result))
            {
                return result;
            }

            var typeResource = resource.GetTypeIdentifier(baseResourceIdentifier);
            var escapedIdentifier = Uri.EscapeDataString(resource.Identifier);
            var resultValue = typeResource.ToString() + ServiceConstants.SEPARATOR_SEGMENTS + escapedIdentifier;

            return new Uri(resultValue);
        }

        public static Uri GetTypeIdentifier(this Schematized schematized, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (schematized.Schemas == null)
            {
                throw new InvalidOperationException(ProtocolResources.ExceptionInvalidResource);
            }

            var path = schematized.GetPath();

            return ComposeTypeIdentifier(baseResourceIdentifier, path);
        }

        public static bool Matches(this IExtension extension, string schemaIdentifier)
        {
            return string.Equals(schemaIdentifier, extension.SchemaIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        internal static IEnumerable<ElectronicMailAddress> PatchElectronicMailAddresses(IEnumerable<ElectronicMailAddress> electronicMailAddresses,
            PatchOperation2 operation)
        {
            if (operation == null)
            {
                return electronicMailAddresses;
            }

            if (!string.Equals(AttributeNames.ELECTRONIC_MAIL_ADDRESSES, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return electronicMailAddresses;
            }

            if (operation.Path.ValuePath == null)
            {
                return electronicMailAddresses;
            }

            if (string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                return electronicMailAddresses;
            }

            var subAttribute = operation.Path.SubAttributes.SingleOrDefault();

            if (subAttribute == null)
            {
                return electronicMailAddresses;
            }

            if ((operation.Value != null && operation.Value.Count != 1) || (operation.Value == null && operation.Name != OperationName.Remove))
            {
                return electronicMailAddresses;
            }

            if (!string.Equals(AttributeNames.TYPE, subAttribute.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return electronicMailAddresses;
            }

            var electronicMailAddressType = subAttribute.ComparisonValue;

            if (!string.Equals(electronicMailAddressType, ElectronicMailAddressBase.HOME, StringComparison.Ordinal)
                && !string.Equals(electronicMailAddressType, ElectronicMailAddressBase.WORK, StringComparison.Ordinal))
            {
                return electronicMailAddresses;
            }

            ElectronicMailAddress electronicMailAddress;
            ElectronicMailAddress electronicMailAddressExisting;

            if (electronicMailAddresses != null)
            {
                electronicMailAddressExisting = electronicMailAddress = electronicMailAddresses.SingleOrDefault(
                    (item) => string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)
                );
            }
            else
            {
                electronicMailAddressExisting = null;
                electronicMailAddress = new ElectronicMailAddress { ItemType = electronicMailAddressType };
            }

            var value = operation.Value?.Single().Value;

            if (value != null && OperationName.Remove == operation.Name)
            {
                if (string.Equals(value, electronicMailAddress.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
                else // when the value not match, should skip remove anything, return the original emails
                {
                    return electronicMailAddresses;
                }
            }

            electronicMailAddress.Value = value;

            IEnumerable<ElectronicMailAddress> result;

            if (string.IsNullOrWhiteSpace(electronicMailAddress.Value))
            {
                if (electronicMailAddressExisting != null)
                {
                    result = electronicMailAddresses.Where(
                        item => !string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)
                    ).ToArray();
                }
                else
                {
                    result = electronicMailAddresses;
                }

                return result;
            }

            if (electronicMailAddressExisting != null)
            {
                return electronicMailAddresses;
            }

            result = new ElectronicMailAddress[] { electronicMailAddress };

            if (electronicMailAddresses == null)
            {
                return result;
            }

            return electronicMailAddresses.Union(electronicMailAddresses).ToArray();
        }

        internal static IEnumerable<Role> PatchRoles(IEnumerable<Role> roles, PatchOperation2 operation)
        {
            if (operation == null)
            {
                return roles;
            }

            if (!string.Equals(AttributeNames.ROLES, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase))
            {
                return roles;
            }

            if (operation.Path.ValuePath == null)
            {
                return roles;
            }

            if (string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                return roles;
            }

            var subAttribute = operation.Path.SubAttributes.SingleOrDefault();

            if (subAttribute == null)
            {
                return roles;
            }

            if ((operation.Value != null && operation.Value.Count != 1) || (operation.Value == null && operation.Name != OperationName.Remove))
            {
                return roles;
            }

            Role role;
            Role roleExisting;

            if (roles != null)
            {
                roleExisting = role = roles.SingleOrDefault(item => string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal));
            }
            else
            {
                roleExisting = null;
                role = new Role() { Primary = true };
            }

            var value = operation.Value?.Single().Value;

            if (value != null && OperationName.Remove == operation.Name && string.Equals(value, role.Value, StringComparison.OrdinalIgnoreCase))
            {
                value = null;
            }

            role.Value = value;

            IEnumerable<Role> result;

            if (string.IsNullOrWhiteSpace(role.Value))
            {
                if (roleExisting != null)
                {
                    result = roles.Where(item => !string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal)).ToArray();
                }
                else
                {
                    result = roles;
                }
                return result;
            }

            if (roleExisting != null)
            {
                return roles;
            }

            result = new Role[] { role };

            if (roles == null)
            {
                return result;
            }

            return roles.Union(roles).ToArray();
        }

        public static IReadOnlyCollection<T> ToCollection<T>(this T item)
        {
            return new T[] { item };
        }

        public static bool References(this PatchRequest2Base<PatchOperation2Combined> patch, string referee)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(referee))
            {
                throw new ArgumentNullException(nameof(referee));
            }

            return patch.TryFindReference(referee, out IReadOnlyCollection<OperationValue> _);
        }

        public static bool TryFindReference(this PatchRequest2Base<PatchOperation2Combined> patch, string referee, out IReadOnlyCollection<OperationValue> references)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            references = null;

            if (string.IsNullOrWhiteSpace(referee))
            {
                throw new ArgumentNullException(nameof(referee));
            }

            var patchOperation2Values = new List<OperationValue>();

            foreach (PatchOperation2Combined operation in patch.Operations)
            {
                OperationValue[] values = null;
                if (operation?.Value != null)
                {
                    values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);
                }

                if (values == null)
                {
                    string value = null;
                    if (operation?.Value != null)
                    {
                        value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value);
                    }

                    var valueSingle = new OperationValue
                    {
                        Value = value
                    };
                    patchOperation2Values.Add(valueSingle);
                }
                else
                {
                    patchOperation2Values.AddRange(values);
                }
            }

            var patchOperationValues = patchOperation2Values.AsReadOnly();
            var referencesBuffer = new List<OperationValue>(patchOperationValues.Count);

            foreach (OperationValue patchOperationValue in patchOperationValues)
            {
                if (!patchOperationValue.TryParseBulkIdentifierReferenceValue(out string value))
                {
                    value = patchOperationValue.Value;
                }

                if (string.Equals(referee, value, StringComparison.InvariantCulture))
                {
                    referencesBuffer.Add(patchOperationValue);
                }
            }

            references = referencesBuffer.ToArray();

            return references.Any();
        }

        private static bool TryParseBulkIdentifierReferenceValue(string value, out string bulkIdentifier)
        {
            bulkIdentifier = null;

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var match = BulkIdentifierExpression.Value.Match(value);
            var result = match.Success;

            if (result)
            {
                bulkIdentifier = match.Groups[EXPRESSION_GROUP_NAME_BULK_IDENTIFIER].Value;
            }

            return result;
        }

        public static bool TryParseBulkIdentifierReferenceValue(this OperationValue value, out string bulkIdentifier)
        {
            bulkIdentifier = null;

            if (value == null)
            {
                return false;
            }

            return TryParseBulkIdentifierReferenceValue(value.Value, out bulkIdentifier);
        }
    }
}
