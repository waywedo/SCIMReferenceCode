// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SCIM;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;

namespace SCIM.Sample.Provider
{
    public class InMemoryGroupProvider : ProviderBase
    {
        private readonly InMemoryStorage _storage;

        public InMemoryGroupProvider()
        {
            _storage = InMemoryStorage.Instance;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new ArgumentException("resource.Identifier");
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new ArgumentException("group.DisplayName");
            }

            IEnumerable<Core2Group> exisitingGroups = _storage.Groups.Values;
            if
            (
                exisitingGroups.Any(
                    (exisitingGroup) =>
                        string.Equals(exisitingGroup.DisplayName, group.DisplayName, StringComparison.Ordinal))
            )
            {
                throw new ResourceConflictException("Group already exists");
            }
            //Update Metadata
            DateTime created = DateTime.UtcNow;
            group.Metadata.Created = created;
            group.Metadata.LastModified = created;

            string resourceIdentifier = Guid.NewGuid().ToString();
            resource.Identifier = resourceIdentifier;
            _storage.Groups.Add(resourceIdentifier, group);

            return Task.FromResult(resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new ArgumentException("resource.Identifier");
            }

            string identifier = resourceIdentifier.Identifier;

            if (_storage.Groups.ContainsKey(identifier))
            {
                _storage.Groups.Remove(identifier);
            }

            return Task.CompletedTask;
        }

        public override Task<QueryResponseBase> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (parameters.AlternateFilters == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
            }

            IEnumerable<Resource> results;

            var queryFilter = parameters.AlternateFilters.SingleOrDefault();
            var predicate = PredicateBuilder.False<Core2Group>();
            var predicateAnd = PredicateBuilder.True<Core2Group>();

            if (queryFilter == null)
            {
                results = _storage.Groups.Values.Cast<Resource>();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
                {
                    throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
                }

                if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
                {
                    throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
                }

                if (queryFilter.FilterOperator != ComparisonOperator.Equals)
                {
                    throw new NotSupportedException(string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, queryFilter.FilterOperator));
                }

                if (queryFilter.AttributePath.Equals(AttributeNames.DISPLAY_NAME))
                {
                    var displayName = queryFilter.ComparisonValue;
                    predicateAnd = predicateAnd.And(p => string.Equals(p.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    throw new NotSupportedException(string.Format(ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, queryFilter.AttributePath));
                }
            }

            predicate = predicate.Or(predicateAnd);
            results = _storage.Groups.Values.Where(predicate.Compile());

            var result = new QueryResponse(results.ToList() as IList<Resource>);

            result.TotalResults = result.ItemsPerPage = results.Count();

            return Task.FromResult(result as QueryResponseBase);
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new ArgumentException("resource.Identifier");
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new ArgumentException("group.DisplayName");
            }

            Core2Group exisitingGroups = resource as Core2Group;
            if
            (
                _storage.Groups.Values.Any((exisitingUser) =>
                    string.Equals(exisitingUser.DisplayName, group.DisplayName, StringComparison.Ordinal) &&
                    !string.Equals(exisitingUser.Identifier, group.Identifier, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                throw new ResourceConflictException("Another group with that displayName already exists");
            }

            if (!_storage.Groups.TryGetValue(group.Identifier, out Core2Group _))
            {
                throw new ResourceNotFoundException();
            }

            // Update metadata
            group.Metadata.Created = exisitingGroups.Metadata.Created;
            group.Metadata.LastModified = DateTime.UtcNow;

            _storage.Groups[group.Identifier] = group;

            var result = group as Resource;

            return Task.FromResult(result);
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string identifier = parameters.ResourceIdentifier.Identifier;

            if (_storage.Groups.ContainsKey(identifier))
            {
                if (_storage.Groups.TryGetValue(identifier, out Core2Group group))
                {
                    Resource result = group as Resource;
                    return Task.FromResult(result);
                }
            }

            throw new ResourceNotFoundException();
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (patch.ResourceIdentifier == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            if (patch.PatchRequest == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            if (patch.PatchRequest is not PatchRequest2 patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            if (_storage.Groups.TryGetValue(patch.ResourceIdentifier.Identifier, out Core2Group group))
            {
                group.Apply(patchRequest);
                // Update metadata
                group.Metadata.LastModified = DateTime.UtcNow;
            }
            else
            {
                throw new ResourceNotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}
