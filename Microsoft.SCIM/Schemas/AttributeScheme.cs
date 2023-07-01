//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class AttributeScheme
    {
        private AttributeDataType _dataType;
        private string _dataTypeValue;
        private Mutability _mutability;
        private string _mutabilityValue;
        private Returned _returned;
        private string _returnedValue;
        private Uniqueness _uniqueness;
        private string _uniquenessValue;
        private List<AttributeScheme> _subAttributes;
        private IReadOnlyCollection<AttributeScheme> _subAttributesWrapper;
        private List<string> _canonicalValues;
        private IReadOnlyCollection<string> _canonicalValuesWrapper;

        private List<string> _referenceTypes;
        private IReadOnlyCollection<string> _referenceTypesWrapper;

        private object _thisLock;

        public AttributeScheme()
        {
        }

        public AttributeScheme(string name, AttributeDataType type, bool plural)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            OnInitialization();
            OnInitialized();
            Name = name;
            DataType = type;
            Plural = plural;
            Mutability = Mutability.readWrite;
            Returned = Returned.@default;
            Uniqueness = Uniqueness.none;
        }

        [DataMember(Name = AttributeNames.CASE_EXACT)]
        public bool CaseExact { get; set; }

        public AttributeDataType DataType
        {
            get { return _dataType; }
            set
            {
                _dataTypeValue = Enum.GetName(typeof(AttributeDataType), value);
                _dataType = value;
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Serialization")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Serialization")]
        [DataMember(Name = AttributeNames.TYPE)]
        private string DataTypeValue
        {
            get { return _dataTypeValue; }
            set
            {
                _dataType = (AttributeDataType)Enum.Parse(typeof(AttributeDataType), value);
                _dataTypeValue = value;
            }
        }

        [DataMember(Name = AttributeNames.DESCRIPTION)]
        public string Description { get; set; }

        public Mutability Mutability
        {
            get { return _mutability; }
            set
            {
                _mutabilityValue = Enum.GetName(typeof(Mutability), value);
                _mutability = value;
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Serialization")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Serialization")]
        [DataMember(Name = AttributeNames.MUTABILITY)]
        private string MutabilityValue
        {
            get { return _mutabilityValue; }
            set
            {
                _mutability = (Mutability)Enum.Parse(typeof(Mutability), value);
                _mutabilityValue = value;
            }
        }

        [DataMember(Name = AttributeNames.NAME)]
        public string Name { get; set; }

        [DataMember(Name = AttributeNames.PLURAL)]
        public bool Plural { get; set; }

        [DataMember(Name = AttributeNames.REQUIRED)]
        public bool Required { get; set; }

        public Returned Returned
        {
            get { return _returned; }
            set
            {
                _returnedValue = Enum.GetName(typeof(Returned), value);
                _returned = value;
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Serialization")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Serialization")]
        [DataMember(Name = AttributeNames.RETURNED)]
        private string ReturnedValue
        {
            get { return _returnedValue; }
            set
            {
                _returned = (Returned)Enum.Parse(typeof(Returned), value);
                _returnedValue = value;
            }
        }

        public Uniqueness Uniqueness
        {
            get { return _uniqueness; }
            set
            {
                _uniquenessValue = Enum.GetName(typeof(Uniqueness), value);
                _uniqueness = value;
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Serialization")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Serialization")]
        [DataMember(Name = AttributeNames.UNIQUENESS)]
        private string UniquenessValue
        {
            get { return _uniquenessValue; }
            set
            {
                _uniqueness = (Uniqueness)Enum.Parse(typeof(Uniqueness), value);
                _uniquenessValue = value;
            }
        }

        [DataMember(Name = AttributeNames.SUB_ATTRIBUTES)]
        public IReadOnlyCollection<AttributeScheme> SubAttributes => _subAttributesWrapper.Count == 0 ? null : _subAttributesWrapper;

        [DataMember(Name = AttributeNames.CANONICAL_VALUES)]
        public IReadOnlyCollection<string> CanonicalValues => _canonicalValuesWrapper.Count == 0 ? null : _canonicalValuesWrapper;

        [DataMember(Name = AttributeNames.REFERENCE_TYPES)]
        public IReadOnlyCollection<string> ReferenceTypes => _referenceTypesWrapper.Count == 0 ? null : _referenceTypesWrapper;

        public void AddSubAttribute(AttributeScheme subAttribute)
        {
            var containsFunction = new Func<bool>(
                () => _subAttributes.Any(item =>
                    string.Equals(item.Name, subAttribute.Name, StringComparison.OrdinalIgnoreCase)
                )
            );

            AddItemFunction(subAttribute, _subAttributes, containsFunction);
        }

        public void AddCanonicalValues(string canonicalValue)
        {
            var containsFunction = new Func<bool>(() =>
                _canonicalValues.Any((item) =>
                    string.Equals(item, canonicalValue, StringComparison.OrdinalIgnoreCase)
                )
            );

            AddItemFunction(canonicalValue, _canonicalValues, containsFunction);
        }

        public void AddReferenceTypes(string referenceType)
        {
            var containsFunction = new Func<bool>(() =>
                _referenceTypes.Any(item =>
                    string.Equals(item, referenceType, StringComparison.OrdinalIgnoreCase)
                )
            );

            AddItemFunction(referenceType, _referenceTypes, containsFunction);
        }

        private void OnInitialization()
        {
            _thisLock = new object();
            _subAttributes = new List<AttributeScheme>();
            _canonicalValues = new List<string>();
            _referenceTypes = new List<string>();
        }

        private void OnInitialized()
        {
            _subAttributesWrapper = _subAttributes.AsReadOnly();
            _canonicalValuesWrapper = _canonicalValues.AsReadOnly();
            _referenceTypesWrapper = _referenceTypes.AsReadOnly();
        }

        private void AddItemFunction<T>(T item, List<T> itemCollection, Func<bool> containsFunction)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!containsFunction())
            {
                lock (_thisLock)
                {
                    if (!containsFunction())
                    {
                        itemCollection.Add(item);
                    }
                }
            }
        }
    }
}
