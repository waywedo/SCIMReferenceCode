//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class PatchOperation2Combined : PatchOperation2Base
    {
        private const string TEMPLATE = "{0}: [{1}]";

        [DataMember(Name = AttributeNames.VALUE, Order = 2)]
        private object _values;

        public PatchOperation2Combined()
        {
        }

        public PatchOperation2Combined(OperationName operationName, string pathExpression) : base(operationName, pathExpression)
        {
        }

        public static PatchOperation2Combined Create(OperationName operationName, string pathExpression, string value)
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

            return new PatchOperation2Combined(operationName, pathExpression)
            {
                Value = JsonConvert.SerializeObject(operationValue)
            };
        }

        public string Value
        {
            get
            {
                if (_values == null)
                {
                    return null;
                }

                return JsonConvert.SerializeObject(_values);
            }

            set
            {
                _values = value;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            if (Value == null && this?.Path?.AttributePath != null
                && Path.AttributePath.Contains(AttributeNames.MEMBERS, StringComparison.OrdinalIgnoreCase)
                && Name == SCIM.OperationName.Remove && Path?.SubAttributes?.Count == 1)
            {
                Value = Path.SubAttributes.First().ComparisonValue;
                Path = SCIM.Path.Create(AttributeNames.MEMBERS);
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