//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class PatchOperation2SingleValued : PatchOperation2Base
    {
        private const string TEMPLATE = "{0}: [{1}]";

        [DataMember(Name = AttributeNames.VALUE, Order = 2)]
        private string _valueValue;

        public PatchOperation2SingleValued()
        {
            OnInitialization();
        }

        public PatchOperation2SingleValued(OperationName operationName, string pathExpression, string value) : base(operationName, pathExpression)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            _valueValue = value;
        }

        public string Value
        {
            get
            {
                return _valueValue;
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _valueValue = string.Empty;
        }

        public override string ToString()
        {
            var operation = base.ToString();

            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, operation, _valueValue);
        }
    }
}
