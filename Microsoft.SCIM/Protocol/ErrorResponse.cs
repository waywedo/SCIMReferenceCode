//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
    using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
    using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class ErrorResponse : Schematized
    {
        private ErrorType _errorType;

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Serialized Value")]
        [DataMember(Name = ProtocolAttributeNames.ERROR_TYPE)]
        private string _errorTypeValue;
        private Response _response;

        public ErrorResponse()
        {
            Initialize();
            AddSchema(ProtocolSchemaIdentifiers.VERSION_2_ERROR);
        }

        [DataMember(Name = ProtocolAttributeNames.DETAIL)]
        public string Detail { get; set; }

        public ErrorType ErrorType
        {
            get { return _errorType; }
            set
            {
                _errorType = value;
                _errorTypeValue = Enum.GetName(typeof(ErrorType), value);
            }
        }

        public HttpStatusCode Status
        {
            get { return _response.Status; }
            set { _response.Status = value; }
        }

        private void Initialize()
        {
            _response = new Response();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            Initialize();
        }
    }
}