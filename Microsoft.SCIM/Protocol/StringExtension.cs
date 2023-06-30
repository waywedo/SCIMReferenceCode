//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.SCIM
{
    internal static class StringExtension
    {
        private const string PATTERN_ESCAPED_DOUBLE_QUOTE = @"\\*" + QUOTE_DOUBLE;
        private const string PATTERN_ESCAPED_SINGLE_QUOTE = @"\\*" + QUOTE_SINGLE;
        private const string QUOTE_DOUBLE = "\"";
        private const string QUOTE_SINGLE = "'";

        private static readonly Lazy<Regex> ExpressionEscapedDoubleQuote =
            new(() => new Regex(PATTERN_ESCAPED_DOUBLE_QUOTE, RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private static readonly Lazy<Regex> ExpressionEscapedSingleQuote =
            new(() => new Regex(PATTERN_ESCAPED_SINGLE_QUOTE, RegexOptions.Compiled | RegexOptions.CultureInvariant));

        public static string Unquote(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var indexQuoteDouble = input.Trim().IndexOf(QUOTE_DOUBLE, 0, StringComparison.OrdinalIgnoreCase);
            var indexQuoteSingle = input.Trim().IndexOf(QUOTE_SINGLE, 0, StringComparison.OrdinalIgnoreCase);
            Regex expression;

            if (indexQuoteDouble == 0)
            {
                expression = ExpressionEscapedDoubleQuote.Value;
            }
            else if (indexQuoteSingle == 0)
            {
                expression = ExpressionEscapedSingleQuote.Value;
            }
            else
            {
                return input;
            }

            var matches = expression.Matches(input);

            if (matches.Count == 0)
            {
                return input;
            }

            var buffer = new StringBuilder(input);

            for (int matchIndex = matches.Count - 1; matchIndex >= 0; matchIndex--)
            {
                var match = matches[matchIndex];
                var index = match.Index;
                buffer.Remove(index, 1);
            }

            return buffer.ToString();
        }
    }
}