//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class Core2GroupBase : GroupBase
    {
        private IDictionary<string, IDictionary<string, object>> _customExtension;

        protected Core2GroupBase()
        {
            AddSchema(SchemaIdentifiers.CORE_2_GROUP);
            Metadata = new Core2Metadata()
            {
                ResourceType = Types.GROUP
            };
            OnInitialization();
        }

        public virtual IReadOnlyDictionary<string, IDictionary<string, object>> CustomExtension
        {
            get
            {
                return new ReadOnlyDictionary<string, IDictionary<string, object>>(_customExtension);
            }
        }

        [DataMember(Name = AttributeNames.METADATA)]
        public Core2Metadata Metadata { get; set; }

        public virtual void AddCustomAttribute(string key, object value)
        {
            if (key?.StartsWith(SchemaIdentifiers.PREFIX_EXTENSION, StringComparison.OrdinalIgnoreCase) == true
                && value is Dictionary<string, object> nestedObject)
            {
                _customExtension.Add(key, nestedObject);
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _customExtension = new Dictionary<string, IDictionary<string, object>>();
        }

        public override Dictionary<string, object> ToJson()
        {
            var result = base.ToJson();

            foreach (var entry in CustomExtension)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }
    }
}