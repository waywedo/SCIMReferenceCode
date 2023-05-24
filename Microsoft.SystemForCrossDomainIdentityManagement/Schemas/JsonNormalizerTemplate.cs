//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.SCIM
{
    public abstract class JsonNormalizerTemplate : IJsonNormalizationBehavior
    {
        public abstract IReadOnlyCollection<string> AttributeNames { get; }

        private IEnumerable<KeyValuePair<string, object>> Normalize(IReadOnlyCollection<KeyValuePair<string, object>> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var countElements = json.CheckedCount();
            var result = new Dictionary<string, object>(countElements);

            foreach (var element in json)
            {
                var key = element.Key;
                var value = element.Value;
                var attributeName = AttributeNames
                    .SingleOrDefault(item => string.Equals(item, key, StringComparison.OrdinalIgnoreCase));

                if (attributeName != null)
                {
                    if (!string.Equals(key, attributeName, StringComparison.Ordinal))
                    {
                        key = attributeName;
                    }

                    switch (value)
                    {
                        case IEnumerable<KeyValuePair<string, object>> jsonValue:
                            value = Normalize(jsonValue);
                            break;
                        case ArrayList jsonCollectionValue:
                            var jsonCollectionNormalized = new ArrayList();

                            foreach (object innerValue in jsonCollectionValue)
                            {
                                if (innerValue is IEnumerable<KeyValuePair<string, object>> innerObject)
                                {
                                    var normalizedInnerObject = Normalize(innerObject);
                                    jsonCollectionNormalized.Add(normalizedInnerObject);
                                }
                                else
                                {
                                    jsonCollectionNormalized.Add(innerValue);
                                }
                            }

                            value = jsonCollectionNormalized;
                            break;
                    }
                }

                result.Add(key, value);
            }

            return result;
        }

        private IEnumerable<KeyValuePair<string, object>> Normalize(IEnumerable<KeyValuePair<string, object>> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var materializedJson = json.ToArray();

            return Normalize(materializedJson);
        }

        public IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var keyedPairs = (IReadOnlyCollection<KeyValuePair<string, object>>)json;
            var normalizedJson = Normalize(keyedPairs).ToDictionary(item => item.Key, item => item.Value);

            return new ReadOnlyDictionary<string, object>(normalizedJson);
        }
    }
}