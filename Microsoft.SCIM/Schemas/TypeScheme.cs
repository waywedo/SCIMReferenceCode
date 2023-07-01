//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class TypeScheme : Resource
    {
        private List<AttributeScheme> _attributes;
        private IReadOnlyCollection<AttributeScheme> _attributesWrapper;
        private object _thisLock;

        public TypeScheme()
        {
            OnInitialization();
            OnInitialized();
            AddSchema(SchemaIdentifiers.CORE_2_SCHEMA);
            Metadata = new Core2Metadata()
            {
                ResourceType = Types.SCHEMA
            };
        }

        [DataMember(Name = AttributeNames.ATTRIBUTES, Order = 0)]
        public IReadOnlyCollection<AttributeScheme> Attributes => _attributesWrapper;

        [DataMember(Name = AttributeNames.NAME)]
        public string Name { get; set; }

        [DataMember(Name = AttributeNames.DESCRIPTION)]
        public string Description { get; set; }

        [DataMember(Name = AttributeNames.METADATA)]
        public Core2Metadata Metadata { get; set; }

        public void AddAttribute(AttributeScheme attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var containsFunction = new Func<bool>(() =>
                _attributes.Any(item => string.Equals(item.Name, attribute.Name, StringComparison.OrdinalIgnoreCase))
            );

            if (!containsFunction())
            {
                lock (_thisLock)
                {
                    if (!containsFunction())
                    {
                        _attributes.Add(attribute);
                    }
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            OnInitialized();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _thisLock = new object();
            _attributes = new List<AttributeScheme>();
        }

        private void OnInitialized()
        {
            _attributesWrapper = _attributes.AsReadOnly();
        }
    }
}
