//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    [DataContract]
    [KnownType(typeof(ErrorResponse))]
    [KnownType(typeof(Core2EnterpriseUser))]
    [KnownType(typeof(QueryResponse<Core2EnterpriseUser>))]
    [KnownType(typeof(Core2User))]
    [KnownType(typeof(QueryResponse<Core2User>))]
    [KnownType(typeof(QueryResponse<Core2Group>))]
    [KnownType(typeof(Core2Group))]
    public sealed class BulkResponseOperation : BulkOperation, IResponse
    {
        private IResponse _response;

        public BulkResponseOperation(string identifier) : base(identifier)
        {
            OnInitialization();
        }

        public BulkResponseOperation()
            : base(null)
        {
            OnInitialization();
        }

        [DataMember(Name = ProtocolAttributeNames.LOCATION)]
        public Uri Location { get; set; }

        [DataMember(Name = ProtocolAttributeNames.RESPONSE)]
        public object Response { get; set; }

        public HttpStatusCode Status
        {
            get => _response.Status;
            set => _response.Status = value;
        }

        [DataMember(Name = ProtocolAttributeNames.STATUS)]
        public string StatusCodeValue
        {
            get => _response.StatusCodeValue;
            set => _response.StatusCodeValue = value;
        }

        public bool IsError()
        {
            return _response.IsError();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _response = new Response();
        }
    }
}
