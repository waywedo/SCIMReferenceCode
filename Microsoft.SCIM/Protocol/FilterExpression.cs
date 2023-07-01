//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    // Parses filter expressions into a doubly-linked list.
    // A collection of IFilter objects can be obtained from the fully-parsed expression.
    //
    // Brackets, that is, '(' and '),' characters demarcate groups.
    // So, each expression has a group identifier.
    // Group identifiers are integers,
    // but the group identifier may be consisted a "nominal variable,"
    // in the terminology of applied statistics: https://en.wikipedia.org/wiki/Level_of_measurement.
    // Specifically, it does not matter that group 4 is followed by group 6,
    // but merely that the expressions of group six are not in group 4.
    //
    // Brackets also demarcate levels.
    // So, each expression has a zero-based level number,
    // zero being the top level.
    // Thus, in the filter expression,
    // a eq 1 and (b eq 2 or c eq 3) and (d eq 4 or e eq 5),
    // the clause, a eq 1,
    // has the level number 0,
    // while the bracketed clauses have the level number 1.
    // The clause, a eq 1 is one group,
    // the first pair of bracketed clauses are in a second group,
    // and the second pair of bracketed clauses are in a third group.
    internal sealed class FilterExpression : IFilterExpression
    {
        private const char BRACKET_CLOSE = ')';
        private const char ESCAPE = '\\';
        private const char QUOTE = '"';

        private const string PATTERN_GROUP_LEFT = "left";
        private const string PATTERN_GROUP_LEVEL_UP = "levelUp";
        private const string PATTERN_GROUP_OPERATOR = "operator";
        private const string PATTERN_GROUP_RIGHT = "right";
        // (?<levelUp>\(*)?(?<left>(\S)*)(\s*(?<operator>bitAnd|eq|ne|co|sw|ew|ge|gt|isMemberOf|lt|matchesExpression|le|notBitAnd|notMatchesExpression)\s*(?<right>(.)*))?
        private const string PATTERN_TEMPLATE = @$"(?<{PATTERN_GROUP_LEVEL_UP}>\(*)?(?<{PATTERN_GROUP_LEFT}>(\S)*)(\s*(?<{PATTERN_GROUP_OPERATOR}>{{0}})\s*(?<{PATTERN_GROUP_RIGHT}>(.)*))?";

        private const char REGULAR_EXPRESSION_OPERATOR_OR = '|';
        private const char SPACE = ' ';
        private const string TEMPLATE = "{0} {1} {2}";

        private static readonly Lazy<char[]> TrailingCharacters = new(() => new char[] { QUOTE, SPACE, BRACKET_CLOSE });
        private static readonly Lazy<string> ComparisonOperators = new(() => Initialize<ComparisonOperatorValue>());
        private static readonly Lazy<string> FilterPattern = new(() => InitializeFilterPattern());
        private static readonly Lazy<Regex> Expression = new(() => new Regex(FilterPattern.Value, RegexOptions.CultureInvariant | RegexOptions.Compiled));
        private static readonly Lazy<string> LogicalOperatorAnd = new(() => Enum.GetName(typeof(LogicalOperatorValue), LogicalOperatorValue.and));
        private static readonly Lazy<string> LogicalOperatorOr = new(() => Enum.GetName(typeof(LogicalOperatorValue), LogicalOperatorValue.or));

        private string _attributePath;
        private ComparisonOperatorValue _comparisonOperator;
        private ComparisonOperator _filterOperator;
        private int _groupValue;
        private int _levelValue;
        private LogicalOperatorValue _logicalOperator;
        private FilterExpression _next;
        private ComparisonValue _value;

        private FilterExpression(FilterExpression other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Text = other.Text;
            _attributePath = other._attributePath;
            _comparisonOperator = other._comparisonOperator;
            _filterOperator = other._filterOperator;
            Group = other.Group;
            Level = other.Level;
            _logicalOperator = other._logicalOperator;
            _value = other._value;

            if (other._next != null)
            {
                _next = new FilterExpression(other._next)
                {
                    Previous = this
                };
            }
        }

        private FilterExpression(string text, int group, int level)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text.Trim();

            Level = level;
            Group = group;

            foreach (var match in Expression.Value.Matches(Text).Cast<Match>())
            {
                var levelUpGroup = match.Groups[PATTERN_GROUP_LEVEL_UP];

                if (levelUpGroup.Success && levelUpGroup.Value.Any())
                {
                    Level += levelUpGroup.Value.Length;
                    Group++;
                }

                var operatorGroup = match.Groups[PATTERN_GROUP_OPERATOR];

                if (operatorGroup.Success)
                {
                    var leftGroup = match.Groups[PATTERN_GROUP_LEFT];
                    var rightGroup = match.Groups[PATTERN_GROUP_RIGHT];
                    Initialize(leftGroup, operatorGroup, rightGroup);
                }
                else
                {
                    var remainder = match.Value.Trim();

                    if (string.IsNullOrWhiteSpace(remainder))
                    {
                    }
                    else if (remainder.Length == 1 && BRACKET_CLOSE == remainder[0])
                    {
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException(remainder, nameof(text));
                    }
                }
            }
        }

        public FilterExpression(string text) : this(text: text, group: 0, level: 0)
        {
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
        }//15

        private interface IComparisonValue
        {
            AttributeDataType DataType { get; }
            bool Quoted { get; }
            string Value { get; }
        }

        private enum LogicalOperatorValue
        {
            and,
            or
        }

        private int Group
        {
            get { return _groupValue; }

            set
            {
                if (value < 0)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);
                    throw new ArgumentOutOfRangeException(message, nameof(Group));
                }
                _groupValue = value;
            }
        }

        private int Level
        {
            get
            {
                return _levelValue;
            }

            set
            {
                if (value < 0)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);
                    throw new ArgumentOutOfRangeException(message, nameof(Level));
                }
                _levelValue = value;
            }
        }

        private ComparisonOperatorValue Operator
        {
            get
            {
                return _comparisonOperator;
            }

            set
            {
                switch (value)
                {
                    case ComparisonOperatorValue.bitAnd:
                        _filterOperator = ComparisonOperator.BitAnd;
                        break;
                    case ComparisonOperatorValue.ew:
                        _filterOperator = ComparisonOperator.EndsWith;
                        break;
                    case ComparisonOperatorValue.sw:
                        _filterOperator = ComparisonOperator.StartsWith;
                        break;
                    case ComparisonOperatorValue.eq:
                        _filterOperator = ComparisonOperator.Equals;
                        break;
                    case ComparisonOperatorValue.ge:
                        _filterOperator = ComparisonOperator.EqualOrGreaterThan;
                        break;
                    case ComparisonOperatorValue.gt:
                        _filterOperator = ComparisonOperator.GreaterThan;
                        break;
                    case ComparisonOperatorValue.le:
                        _filterOperator = ComparisonOperator.EqualOrLessThan;
                        break;
                    case ComparisonOperatorValue.lt:
                        _filterOperator = ComparisonOperator.LessThan;
                        break;
                    case ComparisonOperatorValue.co:
                        _filterOperator = ComparisonOperator.Contains;
                        break;
                    case ComparisonOperatorValue.includes:
                        _filterOperator = ComparisonOperator.Includes;
                        break;
                    case ComparisonOperatorValue.isMemberOf:
                        _filterOperator = ComparisonOperator.IsMemberOf;
                        break;
                    case ComparisonOperatorValue.matchesExpression:
                        _filterOperator = ComparisonOperator.MatchesExpression;
                        break;
                    case ComparisonOperatorValue.notBitAnd:
                        _filterOperator = ComparisonOperator.NotBitAnd;
                        break;
                    case ComparisonOperatorValue.ne:
                        _filterOperator = ComparisonOperator.NotEquals;
                        break;
                    case ComparisonOperatorValue.notMatchesExpression:
                        _filterOperator = ComparisonOperator.NotMatchesExpression;
                        break;
                    default:
                        string notSupported = Enum.GetName(typeof(ComparisonOperatorValue), value);
                        throw new NotSupportedException(notSupported);
                }
                _comparisonOperator = value;
            }
        }

        private FilterExpression Previous { get; set; }
        private string Text { get; }

        private static void And(IFilter left, IFilter right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (left.AdditionalFilter == null)
            {
                left.AdditionalFilter = right;
            }
            else
            {
                And(left.AdditionalFilter, right);
            }
        }

        private static IReadOnlyCollection<IFilter> And(IFilter left, IReadOnlyCollection<IFilter> right)
        {
            var result = new List<IFilter>();
            var template = new Filter(left);

            for (int index = 0; index < right.Count; index++)
            {
                var rightFilter = right.ElementAt(index);
                IFilter leftFilter;

                if (index == 0)
                {
                    leftFilter = left;
                }
                else
                {
                    leftFilter = new Filter(template);
                    result.Add(leftFilter);
                }

                And(leftFilter, rightFilter);
            }

            return result;
        }

        private static IReadOnlyCollection<IFilter> And(IReadOnlyCollection<IFilter> left, IFilter right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            for (int index = 0; index < left.Count; index++)
            {
                IFilter leftFilter = left.ElementAt(index);
                And(leftFilter, right);
            }

            return left;
        }

        // Convert the doubly-linked list into a collection of IFilter objects.
        // There are three cases that may be encountered as the conversion proceeds through the linked list of clauses.
        // Those cases are documented by comments below.
        private IReadOnlyCollection<IFilter> Convert()
        {
            var result = new List<IFilter>();
            var thisFilter = ToFilter();
            result.Add(thisFilter);
            var current = _next;

            while (current != null)
            {
                if (Level == current.Level)
                {
                    // The current clause has the same level number as the initial clause,
                    // such as
                    // b eq 2
                    // in the expression
                    // a eq 1 and b eq 2.
                    var filter = current.ToFilter();
                    switch (current.Previous._logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            var left = result.Last();
                            And(left, filter);
                            break;
                        case LogicalOperatorValue.or:
                            result.Add(filter);
                            break;
                        default:
                            var notSupported = Enum.GetName(typeof(LogicalOperatorValue), _logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }
                    current = current._next;
                }
                else if (Level > current.Level)
                {
                    // The current clause has a lower level number than the initial clause,
                    // such as
                    // c eq 3
                    // in the expression
                    // (a eq 1 and b eq 2) or c eq 3.
                    var superiors = current.Convert();

                    switch (current.Previous._logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            var superior = superiors.First();
                            result = And(result, superior).ToList();
                            result.AddRange(superiors.Skip(1).ToArray());
                            break;
                        case LogicalOperatorValue.or:
                            result.AddRange(superiors);
                            break;
                        default:
                            var notSupported = Enum.GetName(typeof(LogicalOperatorValue), _logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }
                    break;
                }
                else
                {
                    // The current clause has a higher level number than the initial clause,
                    // such as
                    // b eq 2
                    // in the expression
                    // a eq 1 and (b eq 2 or c eq 3) and (d eq 4 or e eq 5)
                    //
                    // In this case, the linked list is edited,
                    // so that
                    // c eq 3
                    // has no next link,
                    // while the next link of
                    // a eq 1
                    // refers to
                    // d eq 4.
                    // Thereby,
                    // b eq 2 or c eq 3
                    // can be converted to filters and combined with the filter composed from
                    // a eq 1,
                    // after which conversion will continue with the conversion of
                    // d eq 4.
                    // It is the change in group number between
                    // c eq 3
                    // and
                    // d eq 4
                    // that identifies the end of current group,
                    // despite the two clauses having the same level number.
                    //
                    // It is because of the editing of the linked list that the public method,
                    // ToFilters(),
                    // makes a copy of the linked list before initiating conversion;
                    // so that,
                    // ToFilters()
                    // can be called on a FilterExpression any number of times,
                    // to yield the same output.
                    var subordinate = current;

                    while (current != null && Level < current.Level && subordinate.Group == current.Group)
                    {
                        current = current._next;
                    }

                    if (current != null)
                    {
                        current.Previous._next = null;
                        subordinate.Previous._next = current;
                    }

                    var subordinates = subordinate.Convert();

                    switch (subordinate.Previous._logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            var superior = result.Last();
                            var merged = And(superior, subordinates);
                            result.AddRange(merged);
                            break;
                        case LogicalOperatorValue.or:
                            result.AddRange(subordinates);
                            break;
                        default:
                            var notSupported = Enum.GetName(typeof(LogicalOperatorValue), _logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }
                }
            }
            return result;
        }

        private void Initialize(Group left, Group @operator, Group right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (@operator == null)
            {
                throw new ArgumentNullException(nameof(@operator));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (!left.Success || !right.Success || string.IsNullOrEmpty(left.Value) || string.IsNullOrEmpty(right.Value))
            {
                var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);

                throw new InvalidOperationException(message);
            }

            _attributePath = left.Value;

            if (!Enum.TryParse(@operator.Value, out ComparisonOperatorValue comparisonOperatorValue))
            {
                var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);

                throw new InvalidOperationException(message);
            }

            Operator = comparisonOperatorValue;

            if (!TryParse(right.Value, out string comparisonValue))
            {
                var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);

                throw new InvalidOperationException(message);
            }

            _value = new ComparisonValue(comparisonValue, QUOTE == right.Value[0]);

            var indexRemainder = right.Value.IndexOf(comparisonValue, StringComparison.Ordinal) + comparisonValue.Length;

            if (indexRemainder >= right.Value.Length)
            {
                return;
            }

            var remainder = right.Value[indexRemainder..];
            var indexAnd = remainder.IndexOf(LogicalOperatorAnd.Value, StringComparison.Ordinal);
            var indexOr = remainder.IndexOf(LogicalOperatorOr.Value, StringComparison.Ordinal);
            int indexNextFilter;
            int indexLogicalOperator;

            if (indexAnd >= 0 && (indexOr < 0 || indexAnd < indexOr))
            {
                indexNextFilter = indexAnd + LogicalOperatorAnd.Value.Length;
                _logicalOperator = LogicalOperatorValue.and;
                indexLogicalOperator = indexAnd;
            }
            else if (indexOr >= 0)
            {
                indexNextFilter = indexOr + LogicalOperatorOr.Value.Length;
                _logicalOperator = LogicalOperatorValue.or;
                indexLogicalOperator = indexOr;
            }
            else
            {
                var tail = remainder.Trim().TrimEnd(TrailingCharacters.Value);

                if (!string.IsNullOrWhiteSpace(tail))
                {
                    var message = string.Format(CultureInfo.InvariantCulture, ProtocolResources.ExceptionInvalidFilterTemplate, Text);

                    throw new InvalidOperationException(message);
                }
                else
                {
                    return;
                }
            }

            var nextExpression = remainder[indexNextFilter..];
            var indexClosingBracket = remainder.IndexOf(BRACKET_CLOSE, StringComparison.InvariantCulture);
            int nextExpressionLevel;
            int nextExpressionGroup;

            if (indexClosingBracket >= 0 && indexClosingBracket < indexLogicalOperator)
            {
                nextExpressionLevel = Level - 1;
                nextExpressionGroup = Group - 1;
            }
            else
            {
                nextExpressionLevel = Level;
                nextExpressionGroup = Group;
            }

            _next = new FilterExpression(nextExpression, nextExpressionGroup, nextExpressionLevel)
            {
                Previous = this
            };
        }

        private static string Initialize<TOperator>()
        {
            var comparisonOperatorValues = Enum.GetValues(typeof(TOperator));
            var buffer = new StringBuilder();

            foreach (TOperator value in comparisonOperatorValues)
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(REGULAR_EXPRESSION_OPERATOR_OR);
                }
                buffer.Append(value);
            }

            return buffer.ToString();
        }

        private static string InitializeFilterPattern()
        {
            return string.Format(CultureInfo.InvariantCulture, PATTERN_TEMPLATE, ComparisonOperators.Value);
        }

        private IFilter ToFilter()
        {
            return new Filter(_attributePath, _filterOperator, _value.Value)
            {
                DataType = _value.DataType
            };
        }

        public IReadOnlyCollection<IFilter> ToFilters()
        {
            return new FilterExpression(this).Convert();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, TEMPLATE, _attributePath, Operator, _value);
        }

        // This function attempts to parse the comparison value out of the text to the right of a given comparison operator.
        // For example, given the expression,
        // a eq 1 and (b eq 2 or c eq 3) and (d eq 4 or e eq 5),
        // the text to the right of the first comparison operator will be,
        // " 1 and (b eq 2 or c eq 3) and (d eq 4 or e eq 5),"
        // and this function should yield "1" as the comparison value.
        //
        // The function aims, first, to correctly parse out arbitrarily complex comparison values that are correctly formatted.
        // Such values may include nested quotes, nested spaces and nested text matching the logical operators, "and" and "or."
        // However, for compatibility with prior behavior, the function also accepts values that are not correctly formatted,
        // but are within expressions that conform to certain assumptions.
        // For example,
        // a = Hello, World!,
        // is accepted,
        // whereas the expression should be,
        // a = "Hello, World!".
        private static bool TryParse(string input, out string comparisonValue)
        {
            comparisonValue = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string buffer;

            if (QUOTE == input[0])
            {
                int index;
                int position = 1;

                while (true)
                {
                    index = input.IndexOf(QUOTE, position);

                    if (index < 0)
                    {
                        throw new InvalidOperationException();
                    }

                    if (index > 1 && ESCAPE == input[index - 1])
                    {
                        position = index + 1;
                        continue;
                    }

                    // If incorrectly-escaped, string comparison values were to be rejected,
                    // which they should be, strictly,
                    // then the following check to verify that the current quote mark is the last character,
                    // or followed by a space or closing bracket,
                    // would not be necessary.
                    // Alas, invalid filters have been accepted in the past.
                    int nextCharacterIndex = index + 1;

                    if (nextCharacterIndex < input.Length && input[nextCharacterIndex] != SPACE && input[nextCharacterIndex] != BRACKET_CLOSE)
                    {
                        position = nextCharacterIndex;
                        continue;
                    }

                    break;
                }

                buffer = input[1..index];
            }
            else
            {
                var index = input.IndexOf(SPACE, StringComparison.InvariantCulture);

                if (index >= 0)
                {
                    // If unquoted string comparison values were to be rejected,
                    // which they should be, strictly,
                    // then the following check to verify that the current space is followed by a logical operator
                    // would not be necessary.
                    // Alas, invalid filters have been accepted in the past.
                    if (input.LastIndexOf(LogicalOperatorAnd.Value, StringComparison.Ordinal) < index
                        && input.LastIndexOf(LogicalOperatorOr.Value, StringComparison.Ordinal) < index)
                    {
                        buffer = input;
                    }
                    else
                    {
                        buffer = input[..index];
                    }
                }
                else
                {
                    buffer = input;
                }
            }

            comparisonValue = QUOTE == input[0] ? buffer : buffer.TrimEnd(TrailingCharacters.Value);

            return true;
        }

        private class ComparisonValue : IComparisonValue
        {
            private const string TEMPLATE = "\"{0}\"";

            public ComparisonValue(string value, bool quoted)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Value = value;
                Quoted = quoted;

                if (Quoted)
                {
                    DataType = AttributeDataType.@string;
                }
                else if (bool.TryParse(Value, out bool _))
                {
                    DataType = AttributeDataType.boolean;
                }
                else if (long.TryParse(Value, out long _))
                {
                    DataType = AttributeDataType.integer;
                }
                else if (double.TryParse(Value, out double _))
                {
                    DataType = AttributeDataType.@decimal;
                }
                else
                {
                    DataType = AttributeDataType.@string;
                }
            }

            public AttributeDataType DataType { get; }

            public bool Quoted { get; }

            public string Value { get; }

            public override string ToString()
            {
                return Quoted ? string.Format(CultureInfo.InvariantCulture, TEMPLATE, Value) : Value;
            }
        }
    }
}