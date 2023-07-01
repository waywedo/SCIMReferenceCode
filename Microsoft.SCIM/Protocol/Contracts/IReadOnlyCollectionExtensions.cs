//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.SCIM.Protocol.Contracts
{

    public static class IReadOnlyCollectionExtensions
    {
        public static IReadOnlyCollection<string> Encode(this IReadOnlyCollection<string> collection)
        {
            return collection.Select(HttpUtility.UrlEncode).ToArray();
        }

        public static bool TryGetPath(this IReadOnlyCollection<IExtension> extensions, string schemaIdentifier, out string path)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            path = null;

            var extension = extensions.SingleOrDefault(
                (item) => string.Equals(schemaIdentifier, item.SchemaIdentifier, StringComparison.OrdinalIgnoreCase)
            );

            if (extension == null)
            {
                return false;
            }

            path = extension.Path;

            return true;
        }
    }
}