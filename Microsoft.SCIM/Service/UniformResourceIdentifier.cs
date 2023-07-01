// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.SCIM.Service
{
    internal class UniformResourceIdentifier : IUniformResourceIdentifier
    {
        private const string ALTERNATE_PATH_TEMPLATE = REGULAR_EXPRESSION_OPERATOR_OR + "{0}";

        private const string ARGUMENT_NAME_IDENTIFIER = "identifier";
        private const string ARGUMENT_NAME_QUERY = "query";

        private const string EXPRESSION_GROUP_NAME_IDENTIFIER = "identifier";
        private const string EXPRESSION_GROUP_NAME_TYPE = "type";

        private const string REGULAR_EXPRESSION_OPERATOR_OR = "|";

        // (?<type>(Groups|Users{0}))/?(?<identifier>[^\?]*)
        // wherein {0} will be replaced with, for example, |MyExtendedTypePath
        private const string RETRIEVAL_PATTERN_TEMPLATE = @$"(?<{EXPRESSION_GROUP_NAME_TYPE}>({ProtocolConstants.PATH_GROUPS}{REGULAR_EXPRESSION_OPERATOR_OR}{ProtocolConstants.PATH_USERS}{{0}}))/?(?<{EXPRESSION_GROUP_NAME_IDENTIFIER}>[^\?]*)";
        private UniformResourceIdentifier(IResourceIdentifier identifier, IResourceQuery query)
        {
            Identifier = identifier ?? throw new ArgumentNullException(ARGUMENT_NAME_IDENTIFIER);
            Query = query ?? throw new ArgumentNullException(ARGUMENT_NAME_QUERY);
            IsQuery = Identifier == null || string.IsNullOrWhiteSpace(Identifier.Identifier);
        }

        public IResourceIdentifier Identifier { get; }

        public bool IsQuery { get; }

        public IResourceQuery Query { get; }

        public static bool TryParse(Uri identifier, IReadOnlyCollection<IExtension> extensions,
            out IUniformResourceIdentifier parsedIdentifier)
        {
            parsedIdentifier = null;

            if (identifier == null)
            {
                throw new ArgumentNullException(ARGUMENT_NAME_IDENTIFIER);
            }

            IReadOnlyCollection<IExtension> effectiveExtensions =
                extensions ?? Array.Empty<IExtension>();

            var queryCollection = new QueryCollection(QueryHelpers.ParseQuery(identifier.Query));
            var query = new ResourceQuery(queryCollection);

            var alternatePathCollection = effectiveExtensions.Select(
                item => string.Format(CultureInfo.InvariantCulture, ALTERNATE_PATH_TEMPLATE, item.Path)
            ).ToArray();
            var alternatePaths = string.Concat(alternatePathCollection);

            var retrievalPattern = string.Format(CultureInfo.InvariantCulture, RETRIEVAL_PATTERN_TEMPLATE, alternatePaths);
            var retrievalExpression = new Regex(retrievalPattern, RegexOptions.IgnoreCase);
            var expressionMatch = retrievalExpression.Match(identifier.AbsoluteUri);

            if (!expressionMatch.Success)
            {
                return false;
            }

            var type = expressionMatch.Groups[EXPRESSION_GROUP_NAME_TYPE].Value;

            if (string.IsNullOrWhiteSpace(type))
            {
                return false;
            }

            string schemaIdentifier;

            switch (type)
            {
                case ProtocolConstants.PATH_GROUPS:
                    schemaIdentifier = SchemaIdentifiers.CORE_2_GROUP;
                    break;
                case ProtocolConstants.PATH_USERS:
                    schemaIdentifier = SchemaIdentifiers.CORE_2_ENTERPRISE_USER;
                    break;
                default:
                    if (extensions == null)
                    {
                        return false;
                    }

                    schemaIdentifier = effectiveExtensions.Where(
                        item => string.Equals(item.Path, type, StringComparison.OrdinalIgnoreCase))
                        .Select(item => item.SchemaIdentifier).SingleOrDefault();

                    if (string.IsNullOrWhiteSpace(schemaIdentifier))
                    {
                        return false;
                    }

                    break;
            }

            var resourceIdentifier = new ResourceIdentifier
            {
                SchemaIdentifier = schemaIdentifier
            };

            var resourceIdentifierValue = expressionMatch.Groups[EXPRESSION_GROUP_NAME_IDENTIFIER].Value;

            if (!string.IsNullOrWhiteSpace(resourceIdentifierValue))
            {
                resourceIdentifier.Identifier = Uri.UnescapeDataString(resourceIdentifierValue);
            }

            parsedIdentifier = new UniformResourceIdentifier(resourceIdentifier, query);

            return true;
        }
    }
}
