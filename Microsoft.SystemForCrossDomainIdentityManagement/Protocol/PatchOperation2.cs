//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class PatchOperation2 : PatchOperation2Base
    {
        private const string TEMPLATE = "{0}: [{1}]";

        [DataMember(Name = AttributeNames.VALUE, Order = 2)]
        private List<OperationValue> _values;
        private IReadOnlyCollection<OperationValue> _valuesWrapper;

        public PatchOperation2()
        {
            OnInitialization();
            OnInitialized();
        }

        public PatchOperation2(OperationName operationName, string pathExpression) : base(operationName, pathExpression)
        {
            OnInitialization();
            OnInitialized();
        }

        public IReadOnlyCollection<OperationValue> Value
        {
            get
            {
                return _valuesWrapper;
            }
        }

        public void AddValue(OperationValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _values.Add(value);
        }

        public static PatchOperation2 Create(OperationName operationName, string pathExpression, string value)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(nameof(pathExpression));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var operationValue = new OperationValue
            {
                Value = value
            };

            var result = new PatchOperation2(operationName, pathExpression);

            result.AddValue(operationValue);

            return result;
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
            _values = new List<OperationValue>();
        }

        private void OnInitialized()
        {
            switch (_values)
            {
                case List<OperationValue> valueList:
                    _valuesWrapper = valueList.AsReadOnly();
                    break;
                default:
                    throw new NotSupportedException(ProtocolResources.ExceptionInvalidValue);
            }
        }

        public override string ToString()
        {
            var allValues = string.Join(Environment.NewLine, Value);
            var operation = base.ToString();

            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, operation, allValues);
        }
    }
}