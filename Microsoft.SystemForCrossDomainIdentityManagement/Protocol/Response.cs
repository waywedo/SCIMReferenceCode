//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Microsoft.SCIM
{
    internal class Response : IResponse
    {
        private readonly object _thisLock = new();
        private HttpResponseClass _responseClass;
        private HttpStatusCode _statusCode;
        private string _statusCodeValue;

        private enum HttpResponseClass
        {
            Informational = 1,
            Success = 2,
            Redirection = 3,
            ClientError = 4,
            ServerError = 5
        }

        public HttpStatusCode Status
        {
            get { return _statusCode; }
            set { StatusCodeValue = ((int)value).ToString(CultureInfo.InvariantCulture); }
        }

        public string StatusCodeValue
        {
            get { return _statusCodeValue; }

            set
            {
                lock (_thisLock)
                {
                    _statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value);
                    _statusCodeValue = value;

                    var responseClassSignifier = _statusCodeValue.First();
                    var responseClassNumber = char.GetNumericValue(responseClassSignifier);
                    var responseClassCode = Convert.ToInt32(responseClassNumber);

                    _responseClass = (HttpResponseClass)Enum.ToObject(typeof(HttpResponseClass), responseClassCode);
                }
            }
        }

        public bool IsError()
        {
            return HttpResponseClass.ClientError == _responseClass
                || HttpResponseClass.ServerError == _responseClass;
        }
    }
}