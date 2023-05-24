//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.SCIM
{
    public sealed class JsonNormalizer : JsonNormalizerTemplate
    {
        private IReadOnlyCollection<string> _attributeNames;

        public override IReadOnlyCollection<string> AttributeNames
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _attributeNames, CollectAttributeNames);
            }
        }

        private static IReadOnlyCollection<string> CollectAttributeNames()
        {
            var attributeNamesType = typeof(AttributeNames);
            var members = attributeNamesType.GetFields(BindingFlags.Public | BindingFlags.Static);

            return members.Select((FieldInfo item) => item.GetValue(null)).Cast<string>().ToArray();
        }
    }
}