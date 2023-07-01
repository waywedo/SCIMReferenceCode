//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Protocol
{

    [DataContract]
    public abstract class PatchOperation2Base : IPatchOperation2Base
    {
        private const string TEMPLATE = "{0} {1}";

        private OperationName _name;
        private string _operationName;
        private IPath _path;

        [DataMember(Name = ProtocolAttributeNames.PATH, Order = 1)]
        private string _pathExpression;

        protected PatchOperation2Base()
        {
        }

        protected PatchOperation2Base(OperationName operationName, string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(nameof(pathExpression));
            }

            Name = operationName;
            Path = Protocol.Path.Create(pathExpression);
        }

        public OperationName Name
        {
            get { return _name; }

            set
            {
                _name = value;
                _operationName = Enum.GetName(typeof(OperationName), value);
            }
        }

        // It's the value of 'op' parameter within the json of request body.
        [DataMember(Name = ProtocolAttributeNames.PATCH_2_OPERATION, Order = 0)]
        public string OperationName
        {
            get
            {
                return _operationName;
            }

            set
            {
                if (!Enum.TryParse(value, true, out _name))
                {
                    throw new NotSupportedException();
                }

                _operationName = value;
            }
        }

        public IPath Path
        {
            get
            {
                if (_path == null && !string.IsNullOrWhiteSpace(_pathExpression))
                {
                    _path = Protocol.Path.Create(_pathExpression);
                }

                return _path;
            }

            set
            {
                _pathExpression = value?.ToString();
                _path = value;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, _operationName, _pathExpression);
        }
    }
}
