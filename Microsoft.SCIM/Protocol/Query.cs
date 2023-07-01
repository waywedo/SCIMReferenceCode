//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public sealed class Query : IQuery
    {
        private const string ATTRIBUTE_NAME_SEPARATOR = ",";

        public IReadOnlyCollection<IFilter> AlternateFilters { get; set; }
        public IReadOnlyCollection<string> ExcludedAttributePaths { get; set; }
        public IPaginationParameters PaginationParameters { get; set; }
        public string Path { get; set; }
        public IReadOnlyCollection<string> RequestedAttributePaths { get; set; }

        public string Compose()
        {
            return ToString();
        }

        private static Filter Clone(IFilter filter, Dictionary<string, string> placeHolders)
        {
            var placeHolder = Guid.NewGuid().ToString();

            placeHolders.Add(placeHolder, filter.ComparisonValueEncoded);

            var result = new Filter(filter.AttributePath, filter.FilterOperator, placeHolder);

            if (filter.AdditionalFilter != null)
            {
                result.AdditionalFilter = Clone(filter.AdditionalFilter, placeHolders);
            }

            return result;
        }

        public override string ToString()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (RequestedAttributePaths?.Any() == true)
            {
                var encodedPaths = RequestedAttributePaths.Encode();
                var requestedAttributes = string.Join(ATTRIBUTE_NAME_SEPARATOR, encodedPaths);

                parameters.Add(QueryKeys.ATTRIBUTES, requestedAttributes);
            }

            if (ExcludedAttributePaths?.Any() == true)
            {
                var encodedPaths = ExcludedAttributePaths.Encode();
                var excludedAttributes = string.Join(ATTRIBUTE_NAME_SEPARATOR, encodedPaths);

                parameters.Add(QueryKeys.EXCLUDED_ATTRIBUTES, excludedAttributes);
            }

            Dictionary<string, string> placeHolders;

            if (AlternateFilters?.Any() == true)
            {
                placeHolders = new Dictionary<string, string>(AlternateFilters.Count);

                var clones = AlternateFilters.Select(item => Clone(item, placeHolders)).ToArray();
                var filters = Filter.ToString(clones);
                var filterParameters = HttpUtility.ParseQueryString(filters);

                foreach (string key in filterParameters.AllKeys)
                {
                    parameters.Add(key, filterParameters[key]);
                }
            }
            else
            {
                placeHolders = new Dictionary<string, string>();
            }

            if (PaginationParameters != null)
            {
                if (PaginationParameters.StartIndex.HasValue)
                {
                    var startIndex = PaginationParameters.StartIndex.Value.ToString(CultureInfo.InvariantCulture);

                    parameters.Add(QueryKeys.START_INDEX, startIndex);
                }

                if (PaginationParameters.Count.HasValue)
                {
                    var count = PaginationParameters.Count.Value.ToString(CultureInfo.InvariantCulture);

                    parameters.Add(QueryKeys.COUNT, count);
                }
            }

            string result = parameters.ToString();

            foreach (KeyValuePair<string, string> placeholder in placeHolders)
            {
                result = result.Replace(placeholder.Key, placeholder.Value, StringComparison.InvariantCulture);
            }

            return result;
        }
    }
}