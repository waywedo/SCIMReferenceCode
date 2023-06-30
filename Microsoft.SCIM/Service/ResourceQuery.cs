// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Microsoft.SCIM
{
    public sealed class ResourceQuery : IResourceQuery
    {
        private const char SEPERATOR_ATTRIBUTES = ',';

        private static readonly Lazy<char[]> SeperatorsAttributes =
            new(() => new char[] { SEPERATOR_ATTRIBUTES });

        public ResourceQuery()
        {
            Filters = Array.Empty<Filter>();
            Attributes = Array.Empty<string>();
            ExcludedAttributes = Array.Empty<string>();
        }

        public ResourceQuery(IReadOnlyCollection<IFilter> filters, IReadOnlyCollection<string> attributes,
            IReadOnlyCollection<string> excludedAttributes)
        {
            Filters = filters ?? throw new ArgumentNullException(nameof(filters));
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            ExcludedAttributes = excludedAttributes ?? throw new ArgumentNullException(nameof(excludedAttributes));
        }

        public ResourceQuery(IQueryCollection query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            foreach (string key in query.Keys)
            {
                if (string.Equals(key, QueryKeys.ATTRIBUTES, StringComparison.OrdinalIgnoreCase))
                {
                    var attributeExpression = query[key];

                    if (!string.IsNullOrWhiteSpace(attributeExpression))
                    {
                        Attributes = ParseAttributes(attributeExpression);
                    }
                }

                if (string.Equals(key, QueryKeys.COUNT, StringComparison.OrdinalIgnoreCase))
                {
                    var action = new Action<IPaginationParameters, int>(
                        (IPaginationParameters pagination, int paginationValue) => pagination.Count = paginationValue
                    );
                    ApplyPaginationParameter(query[key], action);
                }

                if (string.Equals(key, QueryKeys.EXCLUDED_ATTRIBUTES, StringComparison.OrdinalIgnoreCase))
                {
                    var attributeExpression = query[key];

                    if (!string.IsNullOrWhiteSpace(attributeExpression))
                    {
                        ExcludedAttributes = ParseAttributes(attributeExpression);
                    }
                }

                if (string.Equals(key, QueryKeys.FILTER, StringComparison.OrdinalIgnoreCase))
                {
                    var filterExpression = query[key];

                    if (!string.IsNullOrWhiteSpace(filterExpression))
                    {
                        Filters = ParseFilters(filterExpression);
                    }
                }

                if (string.Equals(key, QueryKeys.START_INDEX, StringComparison.OrdinalIgnoreCase))
                {
                    var action = new Action<IPaginationParameters, int>(
                        (IPaginationParameters pagination, int paginationValue) => pagination.StartIndex = paginationValue
                    );
                    ApplyPaginationParameter(query[key], action);
                }
            }

            Filters ??= Array.Empty<Filter>();

            Attributes ??= Array.Empty<string>();

            ExcludedAttributes ??= Array.Empty<string>();
        }

        public IReadOnlyCollection<string> Attributes { get; }

        public IReadOnlyCollection<string> ExcludedAttributes { get; }

        public IReadOnlyCollection<IFilter> Filters { get; }

        public IPaginationParameters PaginationParameters { get; set; }

        private void ApplyPaginationParameter(string value, Action<IPaginationParameters, int> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var parsedValue = int.Parse(value, CultureInfo.InvariantCulture);

            PaginationParameters ??= new PaginationParameters();

            action(PaginationParameters, parsedValue);
        }

        private static IReadOnlyCollection<string> ParseAttributes(string attributeExpression)
        {
            if (string.IsNullOrWhiteSpace(attributeExpression))
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            return attributeExpression
                .Split(SeperatorsAttributes.Value)
                .Select(item => item.Trim()).ToArray();
        }

        private static IReadOnlyCollection<IFilter> ParseFilters(string filterExpression)
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }

            if (!Filter.TryParse(filterExpression, out IReadOnlyCollection<IFilter> results))
            {
                throw new InvalidOperationException();
            }

            return results;
        }
    }
}
