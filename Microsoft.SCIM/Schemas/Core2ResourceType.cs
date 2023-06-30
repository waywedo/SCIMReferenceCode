//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class Core2ResourceType : Resource
    {
        private Uri _endpoint;

        [DataMember(Name = AttributeNames.ENDPOINT)]
        private string _endpointValue;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Serialization")]
        [DataMember(Name = AttributeNames.NAME)]
        private string _name;

        public Core2ResourceType()
        {
            AddSchema(SchemaIdentifiers.CORE_2_RESOURCE_TYPE);
            Metadata = new Core2Metadata()
            {
                ResourceType = Types.RESOURCE_TYPE
            };
        }

        public Uri Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;
                _endpointValue = new SCIMResourceIdentifier(value).RelativePath;
            }
        }

        [DataMember(Name = AttributeNames.METADATA)]
        public Core2Metadata Metadata { get; set; }

        [DataMember(Name = AttributeNames.SCHEMA)]
        public string Schema { get; set; }

        private void InitializeEndpoint(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _endpoint = null;
                return;
            }

            _endpoint = new Uri(value, UriKind.Relative);
        }

        private void InitializeEndpoint()
        {
            InitializeEndpoint(_endpointValue);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            InitializeEndpoint();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext _)
        {
            _name = Identifier;
        }
    }
}