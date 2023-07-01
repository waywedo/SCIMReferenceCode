//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM.Protocol
{
    internal static class DictionaryExtension
    {
        public static void Trim(this IDictionary<string, object> dictionary)
        {
            foreach (string key in dictionary.Keys)
            {
                var value = dictionary[key];

                if (value == null)
                {
                    dictionary.Remove(key);
                }

                if (value is IDictionary<string, object> dictionaryValue)
                {
                    dictionaryValue.Trim();

                    if (dictionaryValue.Count == 0)
                    {
                        dictionary.Remove(key);
                    }
                }
            }
        }
    }
}