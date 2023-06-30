//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.SCIM
{
    public sealed class Path : IPath
    {
        private const string ARGUMENT_NAME_PATH_EXPRESSION = "pathExpression";

        private const string CONSTRUCT_NAME_SUB_ATTRIBUTES = "subAttr";
        private const string CONSTRUCT_NAME_VALUE_PATH = "valuePath";
        private const string PATTERN_TEMPLATE = @"(?<{0}>.*)\[(?<{1}>.*)\]";
        private const string SCHEMA_IDENTIFIER_SUBNAMESPACE = "urn:ietf:params:scim:schemas:";

        private static readonly string Pattern = string.Format(
            CultureInfo.InvariantCulture, PATTERN_TEMPLATE, CONSTRUCT_NAME_VALUE_PATH, CONSTRUCT_NAME_SUB_ATTRIBUTES);

        private static readonly Lazy<string[]> ObsoleteSchemaPrefixPatterns =
            new(() => new string[]
            {
                "urn:scim:schemas:extension:enterprise:1.0.",
                "urn:scim:schemas:extension:enterprise:2.0."
            });

        private static readonly Lazy<Regex> RegularExpression =
            new(() => new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));

        private const char SEPERATOR_ATTRIBUTES = '.';

        private Path(string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_PATH_EXPRESSION);
            }

            Expression = pathExpression;
        }

        public string AttributePath { get; private set; }

        private string Expression { get; }

        public string SchemaIdentifier { get; private set; }

        public IReadOnlyCollection<IFilter> SubAttributes { get; private set; }

        public IPath ValuePath { get; private set; }

        public static IPath Create(string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_PATH_EXPRESSION);
            }

            if (!TryParse(pathExpression, out IPath result))
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ProtocolResources.ExceptionInvalidPathTemplate, pathExpression);

                throw new ArgumentException(exceptionMessage);
            }

            return result;
        }

        private static bool TryExtractSchemaIdentifier(string pathExpression, out string schemaIdentifier)
        {
            schemaIdentifier = null;

            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                return false;
            }

            if (!pathExpression.StartsWith(SCHEMA_IDENTIFIER_SUBNAMESPACE, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            foreach (var item in ObsoleteSchemaPrefixPatterns.Value)
            {
                if (pathExpression.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            var seperatorIndex = pathExpression.LastIndexOf(SchemaConstants.SEPARATOR_SCHEMA_IDENTIFIER_ATTRIBUTE, StringComparison.OrdinalIgnoreCase);

            if (-1 == seperatorIndex)
            {
                return false;
            }

            schemaIdentifier = pathExpression[..seperatorIndex];

            return true;
        }

        public static bool TryParse(string pathExpression, out IPath path)
        {
            path = null;

            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_PATH_EXPRESSION);
            }

            var buffer = new Path(pathExpression);
            string expression = pathExpression;

            if (TryExtractSchemaIdentifier(pathExpression, out string schemaIdentifier))
            {
                expression = expression[(schemaIdentifier.Length + 1)..];
                buffer.SchemaIdentifier = schemaIdentifier;
            }

            int seperatorIndex = expression.IndexOf(SEPERATOR_ATTRIBUTES, StringComparison.InvariantCulture);
            if (seperatorIndex >= 0)
            {
                var valuePathExpression = expression[(seperatorIndex + 1)..];

                expression = expression[..seperatorIndex];

                if (!TryParse(valuePathExpression, out IPath valuePath))
                {
                    return false;
                }

                buffer.ValuePath = valuePath;
                buffer.SubAttributes = Array.Empty<IFilter>();
            }

            var match = RegularExpression.Value.Match(expression);

            if (!match.Success)
            {
                buffer.AttributePath = expression;
                buffer.SubAttributes = Array.Empty<IFilter>();
            }
            else
            {
                buffer.AttributePath = match.Groups[CONSTRUCT_NAME_VALUE_PATH].Value;

                var filterExpression = match.Groups[CONSTRUCT_NAME_SUB_ATTRIBUTES].Value;

                if (!Filter.TryParse(filterExpression, out IReadOnlyCollection<IFilter> filters))
                {
                    return false;
                }

                buffer.SubAttributes = filters;
            }

            path = buffer;

            return true;
        }

        public override string ToString()
        {
            return Expression;
        }
    }
}