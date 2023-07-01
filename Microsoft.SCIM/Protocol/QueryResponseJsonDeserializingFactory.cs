//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    public sealed class QueryResponseJsonDeserializingFactory<T> : ProtocolJsonDeserializingFactory<QueryResponse<T>>
        where T : Resource
    {
        public QueryResponseJsonDeserializingFactory(JsonDeserializingFactory<Schematized> jsonDeserializingFactory)
        {
            JsonDeserializingFactory =
                jsonDeserializingFactory ?? throw new ArgumentNullException(nameof(jsonDeserializingFactory));
        }

        private JsonDeserializingFactory<Schematized> JsonDeserializingFactory { get; }

        public override QueryResponse<T> Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (!typeof(T).IsAbstract)
            {
                return base.Create(json);
            }
            else
            {
                var normalizedJson = Normalize(json);
                var metadataJson = normalizedJson.Where(
                    (item) => !string.Equals(ProtocolAttributeNames.RESOURCES,
                        item.Key, StringComparison.OrdinalIgnoreCase)
                ).ToDictionary(
                    (item) => item.Key,
                    (item) => item.Value
                );

                var result = base.Create(metadataJson);

                var resourcesJson = normalizedJson.Where(
                    (item) => string.Equals(ProtocolAttributeNames.RESOURCES,
                    item.Key, StringComparison.OrdinalIgnoreCase)).ToArray();

                if (resourcesJson.Any())
                {
                    var resourcesArray = (IEnumerable)resourcesJson.Single().Value;
                    var resources = new List<T>(result.TotalResults);

                    foreach (object element in resourcesArray)
                    {
                        var resourceJson = (IReadOnlyDictionary<string, object>)element;
                        var resource = (T)JsonDeserializingFactory.Create(resourceJson);
                        resources.Add(resource);
                    }

                    result.Resources = resources;
                }

                return result;
            }
        }
    }
}