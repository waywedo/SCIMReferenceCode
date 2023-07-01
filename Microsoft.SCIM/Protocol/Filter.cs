//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    public sealed class Filter : IFilter
    {
        private const string COMPARISON_VALUE_TEMPLATE = "\"{0}\"";

        private const string ENCODING_SPACE_PER_2396 = "+";

        public const string NULL_VALUE = "null";

        private const string RESERVED_PER_RFC_2396 = ";/?:@&=+$,";
        private const string RESERVED_PER_RFC_3986 = RESERVED_PER_RFC_2396 + "#[]!'()*";

        private const string SPACE = " ";

        private const string TEMPLATE = "filter={0}";
        private const string TEMPLATE_COMPARISON = "{0} {1} {2}";
        private const string TEMPLATE_CONJUNCTION = "{0} {1} {2}";
        private const string TEMPLATE_NESTING = "({0})";

        private static readonly Lazy<char[]> ReservedCharactersPerRfc3986 = new(() => RESERVED_PER_RFC_3986.ToCharArray());
        private static readonly Lazy<IReadOnlyDictionary<string, string>> ReservedCharacterEncodingsPerRfc3986 = new(() => InitializeReservedCharacter3986Encodings());
        private static readonly Lazy<IReadOnlyDictionary<string, string>> ReservedCharacterEncodingsPerRfc2396 = new(() => InitializeReservedCharacter2396Encodings());

        private string _comparisonValue;
        private string _comparisonValueEncoded;
        private AttributeDataType? _dataType;

        private Filter()
        {
        }

        public Filter(string attributePath, ComparisonOperator filterOperator, string comparisonValue)
        {
            if (string.IsNullOrWhiteSpace(attributePath))
            {
                throw new ArgumentNullException(nameof(attributePath));
            }

            if (string.IsNullOrWhiteSpace(comparisonValue))
            {
                throw new ArgumentNullException(nameof(comparisonValue));
            }

            AttributePath = attributePath;
            FilterOperator = filterOperator;
            ComparisonValue = comparisonValue;
            DataType = AttributeDataType.@string;
        }

        public Filter(IFilter other) : this(other?.AttributePath, other.FilterOperator, other?.ComparisonValue)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            DataType = other.DataType;

            if (other.AdditionalFilter != null)
            {
                AdditionalFilter = new Filter(other.AdditionalFilter);
            }
        }

        private enum ComparisonOperatorValue
        {
            bitAnd,
            eq,
            ne,
            co,
            sw,
            ew,
            ge,
            gt,
            includes,
            isMemberOf,
            lt,
            matchesExpression,
            le,
            notBitAnd,
            notMatchesExpression
        }

        private enum LogicalOperatorValue
        {
            and,
            or
        }

        public IFilter AdditionalFilter { get; set; }

        public string AttributePath { get; }

        public string ComparisonValue
        {
            get
            {
                return _comparisonValue;
            }
            private set
            {
                Validate(DataType, value);

                _comparisonValue = value;

                var encodedValue = _comparisonValue;

                foreach (KeyValuePair<string, string> encoding in ReservedCharacterEncodingsPerRfc2396.Value)
                {
                    encodedValue = encodedValue.Replace(encoding.Key, encoding.Value, StringComparison.InvariantCulture);
                }

                _comparisonValueEncoded = encodedValue;
            }
        }

        public string ComparisonValueEncoded { get { return _comparisonValueEncoded; } }

        public AttributeDataType? DataType
        {
            get { return _dataType; }
            set
            {
                Validate(value, ComparisonValue);
                _dataType = value;
            }
        }

        public ComparisonOperator FilterOperator { get; set; }

        public static Lazy<IReadOnlyDictionary<string, string>> ReservedCharacterEncodingsPerRfc39861 => ReservedCharacterEncodingsPerRfc3986;

        private static IReadOnlyDictionary<string, string> InitializeReservedCharacter2396Encodings()
        {
            var result = ReservedCharacterEncodingsPerRfc39861.Value
                .ToDictionary(
                    (item) => item.Key,
                    (item) => item.Value
                );

            result.Add(SPACE, ENCODING_SPACE_PER_2396);

            return result;
        }

        private static IReadOnlyDictionary<string, string> InitializeReservedCharacter3986Encodings()
        {
            var result = new Dictionary<string, string>(ReservedCharactersPerRfc3986.Value.Length);

            foreach (char character in ReservedCharactersPerRfc3986.Value)
            {
                var from = character.ToString(CultureInfo.InvariantCulture);
                var to = HttpUtility.UrlEncode(from);

                result.Add(from, to);
            }

            return result;
        }

        public string Serialize()
        {
            ComparisonOperatorValue operatorValue;

            switch (FilterOperator)
            {
                case ComparisonOperator.BitAnd:
                    operatorValue = ComparisonOperatorValue.bitAnd;
                    break;
                case ComparisonOperator.EndsWith:
                    operatorValue = ComparisonOperatorValue.ew;
                    break;
                case ComparisonOperator.Equals:
                    operatorValue = ComparisonOperatorValue.eq;
                    break;
                case ComparisonOperator.EqualOrGreaterThan:
                    operatorValue = ComparisonOperatorValue.ge;
                    break;
                case ComparisonOperator.GreaterThan:
                    operatorValue = ComparisonOperatorValue.gt;
                    break;
                case ComparisonOperator.EqualOrLessThan:
                    operatorValue = ComparisonOperatorValue.le;
                    break;
                case ComparisonOperator.LessThan:
                    operatorValue = ComparisonOperatorValue.lt;
                    break;
                case ComparisonOperator.Includes:
                    operatorValue = ComparisonOperatorValue.includes;
                    break;
                case ComparisonOperator.IsMemberOf:
                    operatorValue = ComparisonOperatorValue.isMemberOf;
                    break;
                case ComparisonOperator.MatchesExpression:
                    operatorValue = ComparisonOperatorValue.matchesExpression;
                    break;
                case ComparisonOperator.NotBitAnd:
                    operatorValue = ComparisonOperatorValue.notBitAnd;
                    break;
                case ComparisonOperator.NotEquals:
                    operatorValue = ComparisonOperatorValue.ne;
                    break;
                case ComparisonOperator.NotMatchesExpression:
                    operatorValue = ComparisonOperatorValue.notMatchesExpression;
                    break;
                default:
                    string notSupportedValue = Enum.GetName(typeof(ComparisonOperator), FilterOperator);
                    throw new NotSupportedException(notSupportedValue);
            }

            string rightHandSide;

            switch (DataType ?? AttributeDataType.@string)
            {
                case AttributeDataType.boolean:
                case AttributeDataType.@decimal:
                case AttributeDataType.integer:
                    rightHandSide = ComparisonValue;
                    break;
                default:
                    rightHandSide = string.Format(CultureInfo.InvariantCulture, COMPARISON_VALUE_TEMPLATE, ComparisonValue);
                    break;
            }

            var filter = string.Format(CultureInfo.InvariantCulture, TEMPLATE_COMPARISON, AttributePath, operatorValue, rightHandSide);

            if (AdditionalFilter != null)
            {
                var additionalFilter = AdditionalFilter.Serialize();

                return string.Format(CultureInfo.InvariantCulture, TEMPLATE_CONJUNCTION, filter, LogicalOperatorValue.and, additionalFilter);
            }
            else
            {
                return filter;
            }
        }

        public override string ToString()
        {
            return Serialize();
        }

        public static string ToString(IReadOnlyCollection<IFilter> filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var placeholder = Guid.NewGuid().ToString();
            string allFilters = null;

            foreach (IFilter filter in filters)
            {
                var clone = new Filter(filter)
                {
                    ComparisonValue = placeholder
                };
                var currentFilter = clone.Serialize();
                var encodedFilter = HttpUtility.UrlEncode(currentFilter).Replace(placeholder, filter.ComparisonValueEncoded, StringComparison.InvariantCulture);

                if (string.IsNullOrWhiteSpace(allFilters))
                {
                    allFilters = filters.Count > 1 ? string.Format(CultureInfo.InvariantCulture, TEMPLATE_NESTING, encodedFilter) : encodedFilter;
                }
                else
                {
                    var rightHandSide = filter.AdditionalFilter != null || filters.Count > 1
                        ? string.Format(CultureInfo.InvariantCulture, TEMPLATE_NESTING, encodedFilter)
                        : encodedFilter;

                    allFilters = string.Format(CultureInfo.InvariantCulture, TEMPLATE_CONJUNCTION, allFilters, LogicalOperatorValue.or, rightHandSide);
                }
            }

            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, allFilters);
        }

        public static bool TryParse(string filterExpression, out IReadOnlyCollection<IFilter> filters)
        {
            var expression = filterExpression?.Trim()?.Unquote();

            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }

            try
            {
                filters = new FilterExpression(expression).ToFilters();
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                filters = null;
                return false;
            }
            catch (ArgumentException)
            {
                filters = null;
                return false;
            }
            catch (InvalidOperationException)
            {
                filters = null;
                return false;
            }
        }

        private static void Validate(AttributeDataType? dataType, string value)
        {
            if (!dataType.HasValue || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            switch (dataType.Value)
            {
                case AttributeDataType.boolean:
                    if (!bool.TryParse(value, out bool _))
                    {
                        throw new InvalidOperationException(ProtocolResources.ExceptionInvalidValue);
                    }
                    break;
                case AttributeDataType.@decimal:
                    if (!double.TryParse(value, out double _))
                    {
                        throw new InvalidOperationException(ProtocolResources.ExceptionInvalidValue);
                    }
                    break;
                case AttributeDataType.integer:
                    if (!long.TryParse(value, out long _))
                    {
                        throw new InvalidOperationException(ProtocolResources.ExceptionInvalidValue);
                    }
                    break;
                case AttributeDataType.binary:
                case AttributeDataType.complex:
                case AttributeDataType.dateTime:
                case AttributeDataType.reference:
                case AttributeDataType.@string:
                    break;
                default:
                    string unsupported = Enum.GetName(typeof(AttributeDataType), dataType.Value);
                    throw new NotSupportedException(unsupported);
            }
        }
    }
}