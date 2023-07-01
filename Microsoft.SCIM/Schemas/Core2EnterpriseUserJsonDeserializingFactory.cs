//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SCIM.Schemas
{
    public sealed class Core2EnterpriseUserJsonDeserializingFactory : JsonDeserializingFactory<Core2EnterpriseUser>
    {
        private static readonly Lazy<JsonDeserializingFactory<Manager>> _managerFactory =
            new(() => new ManagerDeserializingFactory());

        public override Core2EnterpriseUser Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var normalizedJson = Normalize(json);
            Manager manager;
            IReadOnlyDictionary<string, object> safeJson;

            if (normalizedJson.TryGetValue(AttributeNames.MANAGER, out object managerData) && managerData != null)
            {
                safeJson = normalizedJson
                    .Where(item => !string.Equals(AttributeNames.MANAGER, item.Key, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(item => item.Key, item => item.Value);

                switch (managerData)
                {
                    case string value:
                        manager = new Manager() { Value = value };
                        break;
                    case Dictionary<string, object> managerJson:
                        manager = _managerFactory.Value.Create(managerJson);
                        break;
                    default:
                        throw new NotSupportedException(managerData.GetType().FullName);
                }
            }
            else
            {
                safeJson = normalizedJson.ToDictionary(item => item.Key, item => item.Value);
                manager = null;
            }

            var result = base.Create(safeJson);

            foreach (KeyValuePair<string, object> entry in json)
            {
                if (entry.Key.StartsWith(SchemaIdentifiers.PREFIX_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !entry.Key.StartsWith(SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.OrdinalIgnoreCase)
                    && entry.Value is Dictionary<string, object> nestedObject)
                {
                    result.AddCustomAttribute(entry.Key, nestedObject);
                }
            }

            return result;
        }

        private class ManagerDeserializingFactory : JsonDeserializingFactory<Manager>
        {
        }
    }
}