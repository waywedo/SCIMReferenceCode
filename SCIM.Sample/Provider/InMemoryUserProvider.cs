// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class InMemoryUserProvider : ProviderBase
    {
        private readonly InMemoryStorage _storage;

        public InMemoryUserProvider()
        {
            _storage = InMemoryStorage.Instance;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new ArgumentException("resource.Identifier");
            }

            var user = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException(nameof(user.UserName));
            }

            var existingUsers = _storage.Users.Values;

            if (existingUsers.Any((e) => string.Equals(e.UserName, user.UserName, StringComparison.Ordinal)))
            {
                throw new ResourceConflictException("User already exists");
            }

            // Update metadata
            var created = DateTime.UtcNow;
            user.Metadata.Created = created;
            user.Metadata.LastModified = created;

            string resourceIdentifier = Guid.NewGuid().ToString();
            resource.Identifier = resourceIdentifier;

            _storage.Users.Add(resourceIdentifier, user);

            return Task.FromResult(resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new ArgumentException("resource.Identifier");
            }

            var identifier = resourceIdentifier.Identifier;

            if (_storage.Users.ContainsKey(identifier))
            {
                _storage.Users.Remove(identifier);
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
            Expression<Func<Core2EnterpriseUser, bool>> predicateAnd;
            var predicate = PredicateBuilder.False<Core2EnterpriseUser>();

            if (parameters.AlternateFilters.Count == 0)
            {
                results = _storage.Users.Values.Cast<Resource>();
            }
            else
            {
                foreach (var queryFilter in parameters.AlternateFilters)
                {
                    predicateAnd = PredicateBuilder.True<Core2EnterpriseUser>();

                    var andFilter = queryFilter;
                    var currentFilter = andFilter;

                    do
                    {
                        if (string.IsNullOrWhiteSpace(andFilter.AttributePath))
                        {
                            throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
                        }
                        else if (string.IsNullOrWhiteSpace(andFilter.ComparisonValue))
                        {
                            throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
                        }
                        // UserName filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.USER_NAME, StringComparison.OrdinalIgnoreCase))
                        {
                            var userName = andFilter.ComparisonValue;

                            if (andFilter.FilterOperator == ComparisonOperator.Equals)
                            {
                                predicateAnd = predicateAnd.And(p => string.Equals(p.UserName, userName, StringComparison.OrdinalIgnoreCase));
                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.StartsWith)
                            {
                                predicateAnd = predicateAnd.And(p => p.UserName.StartsWith(userName, StringComparison.OrdinalIgnoreCase));
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }
                        }
                        // FamilyName filter
                        else if (andFilter.AttributePath.Equals(string.Concat(AttributeNames.NAME, ".", AttributeNames.FAMILY_NAME), StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var familyName = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.Name.FamilyName, familyName, StringComparison.OrdinalIgnoreCase));
                        }
                        // Email filter
                        else if (andFilter.AttributePath.Equals(string.Concat(AttributeNames.ELECTRONIC_MAIL_ADDRESSES, ".", AttributeNames.VALUE), StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Contains)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var email = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => p.ElectronicMailAddresses.Any(e => e.Value.Contains(email, StringComparison.OrdinalIgnoreCase)));
                        }
                        // ID filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var id = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.Identifier, id, StringComparison.OrdinalIgnoreCase));
                        }
                        // DisplayName filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.DISPLAY_NAME, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var displayName = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
                        }
                        // ExternalId filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.EXTERNAL_IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var externalIdentifier = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.ExternalIdentifier, externalIdentifier, StringComparison.OrdinalIgnoreCase));
                        }
                        //Active Filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.ACTIVE, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }

                            var active = bool.Parse(andFilter.ComparisonValue);
                            predicateAnd = predicateAnd.And(p => p.Active == active);
                        }
                        //LastModified filter
                        else if (andFilter.AttributePath.Equals($"{AttributeNames.METADATA}.{AttributeNames.LAST_MODIFIED}", StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator == ComparisonOperator.EqualOrGreaterThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified >= comparisonValue);
                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.EqualOrLessThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified <= comparisonValue);
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }
                        }
                        //Created filter
                        else if (andFilter.AttributePath.Equals($"{AttributeNames.METADATA}.{AttributeNames.CREATED}", StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator == ComparisonOperator.EqualOrGreaterThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified >= comparisonValue);
                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.GreaterThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified > comparisonValue);
                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.EqualOrLessThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified <= comparisonValue);
                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.LessThan)
                            {
                                var comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified < comparisonValue);
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    string.Format(ServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator)
                                );
                            }
                        }
                        else
                        {
                            throw new NotSupportedException(
                                string.Format(ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, andFilter.AttributePath)
                            );
                        }

                        currentFilter = andFilter;
                        andFilter = andFilter.AdditionalFilter;
                    } while (currentFilter.AdditionalFilter != null);

                    predicate = predicate.Or(predicateAnd);
                }

                results = _storage.Users.Values.Where(predicate.Compile());
            }

            if (parameters.PaginationParameters != null)
            {
                var count = parameters.PaginationParameters.Count ?? 0;
                var result = new QueryResponse(results.Take(count).ToList() as IList<Resource>)
                {
                    ItemsPerPage = parameters.PaginationParameters.Count ?? results.Count(),
                    TotalResults = results.Count()
                };

                return Task.FromResult(result as QueryResponseBase);
            }
            else
            {
                var result = new QueryResponse(results.ToList() as IList<Resource>);

                result.TotalResults = result.ItemsPerPage = results.Count();

                return Task.FromResult(result as QueryResponseBase);
            }
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new ArgumentException("resource.Identifier");
            }

            var user = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException(nameof(user.UserName));
            }

            if
            (
                _storage.Users.Values.Any(
                    (existingUser) =>
                        string.Equals(existingUser.UserName, user.UserName, StringComparison.Ordinal) &&
                        !string.Equals(existingUser.Identifier, user.Identifier, StringComparison.OrdinalIgnoreCase))
            )
            {
                throw new ResourceConflictException("Another user with that userName already exists");
            }

            var existingUser = _storage.Users.Values.FirstOrDefault((existingUser) =>
                string.Equals(existingUser.Identifier, user.Identifier, StringComparison.OrdinalIgnoreCase)
            ) ?? throw new ResourceNotFoundException();

            // Update metadata
            user.Metadata.Created = existingUser.Metadata.Created;
            user.Metadata.LastModified = DateTime.UtcNow;

            _storage.Users[user.Identifier] = user;

            return Task.FromResult(user as Resource);
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

            var identifier = parameters.ResourceIdentifier.Identifier;

            if (_storage.Users.TryGetValue(identifier, out Core2EnterpriseUser user))
            {
                return Task.FromResult(user as Resource);
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
                throw new ArgumentException(string.Format(ServiceResources.ExceptionInvalidOperation));
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
                var unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            if (_storage.Users.TryGetValue(patch.ResourceIdentifier.Identifier, out Core2EnterpriseUser user))
            {
                user.Apply(patchRequest);

                // Update metadata
                user.Metadata.LastModified = DateTime.UtcNow;
            }
            else
            {
                throw new ResourceNotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}
