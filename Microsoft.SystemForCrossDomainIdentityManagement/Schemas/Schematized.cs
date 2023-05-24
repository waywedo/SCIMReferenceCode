//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class Schematized : IJsonSerializable
    {
        [DataMember(Name = AttributeNames.SCHEMAS, Order = 0)]
        private List<string> _schemas;
        private IReadOnlyCollection<string> _schemasWrapper;
        private object _thisLock;
        private IJsonSerializable _serializer;

        protected Schematized()
        {
            OnInitialization();
            OnInitialized();
        }

        public virtual IReadOnlyCollection<string> Schemas { get { return _schemasWrapper; } }

        public void AddSchema(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            var containsFunction = new Func<bool>(
                () => _schemas.Any(item => string.Equals(item, schemaIdentifier, StringComparison.OrdinalIgnoreCase))
            );

            if (!containsFunction())
            {
                lock (_thisLock)
                {
                    if (!containsFunction())
                    {
                        _schemas.Add(schemaIdentifier);
                    }
                }
            }
        }

        public bool Is(string scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            return _schemas.Any(item => string.Equals(item, scheme, StringComparison.OrdinalIgnoreCase));
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
            _serializer = new JsonSerializer(this);
            _schemas = new List<string>();
        }

        private void OnInitialized()
        {
            _schemasWrapper = _schemas.AsReadOnly();
        }

        public virtual Dictionary<string, object> ToJson()
        {
            return _serializer.ToJson();
        }

        public virtual string Serialize()
        {
            var json = ToJson();

            return JsonFactory.Instance.Create(json, true);
        }

        public override string ToString()
        {
            return Serialize();
        }

        public virtual bool TryGetPath(out string path)
        {
            path = null;
            return false;
        }

        public virtual bool TryGetSchemaIdentifier(out string schemaIdentifier)
        {
            schemaIdentifier = null;
            return false;
        }
    }
}