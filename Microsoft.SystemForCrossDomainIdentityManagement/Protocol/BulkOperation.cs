//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class BulkOperation
    {
        private HttpMethod _method;
        private string _methodName;

        protected BulkOperation()
        {
            Identifier = Guid.NewGuid().ToString();
        }

        protected BulkOperation(string identifier)
        {
            Identifier = identifier;
        }

        [DataMember(Name = ProtocolAttributeNames.BULK_OPERATION_IDENTIFIER, Order = 1)]
        public string Identifier { get; private set; }

        public HttpMethod Method
        {
            get { return _method; }
            set
            {
                _method = value;

                if (value != null)
                {
                    _methodName = value.ToString();
                }
            }
        }

        [DataMember(Name = ProtocolAttributeNames.METHOD, Order = 0)]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Required for protocol")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Required for protocol")]
        private string MethodName
        {
            get => _methodName;

            set
            {
                _method = new HttpMethod(value);
                _methodName = value;
            }
        }
    }
}
