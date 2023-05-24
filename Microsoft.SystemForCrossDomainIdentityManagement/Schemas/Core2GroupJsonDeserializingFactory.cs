//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public sealed class Core2GroupJsonDeserializingFactory : JsonDeserializingFactory<Core2Group>
    {
        public override Core2Group Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var result = base.Create(json);

            foreach (var entry in json)
            {
                if (entry.Key.StartsWith(SchemaIdentifiers.PREFIX_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && entry.Value is Dictionary<string, object> nestedObject)
                {
                    result.AddCustomAttribute(entry.Key, nestedObject);
                }
            }

            return result;
        }
    }
}