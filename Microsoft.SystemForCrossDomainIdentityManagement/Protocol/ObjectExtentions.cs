//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.SCIM
{
    public static class ObjectExtentions
    {
        public static bool IsResourceType(this object json, string scheme)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }
            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(json.ToString());
            bool result = false;

            if (operationDataJson.schemas is JArray schemas)
            {
                var schemasList = schemas.ToObject<string[]>();
                result = schemasList.Any((string item) => string.Equals(item, scheme, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }
    }
}
