// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;
using Newtonsoft.Json;

namespace Microsoft.SCIM.Service
{
    public sealed class SampleProvider : ProviderBase, ISampleProvider
    {
        public const string ELECTRONIC_MAIL_ADDRESS_HOME = "babs@jensen.org";
        public const string ELECTRONIC_MAIL_ADDRESS_WORK = "bjensen@example.com";

        public const string EXTENSION_ATTRIBUTE_ENTERPRISE_USER_COST_CENTER = "4130";
        public const string EXTENSION_ATTRIBUTE_ENTERPRISE_USER_DEPARTMENT = "Tour Operations";
        public const string EXTENSION_ATTRIBUTE_ENTERPRISE_USER_DIVISION = "Theme Park";
        public const string EXTENSION_ATTRIBUTE_ENTERPRISE_USER_EMPLOYEE_NUMBER = "701984";
        public const string EXTENSION_ATTRIBUTE_ENTERPRISE_USER_ORGANIZATION = "Universal Studios";

        public const string GROUP_NAME = "Creative & Skinning";
        public const string IDENTIFIER_GROUP = "acbf3ae7-8463-4692-b4fd-9b4da3f908ce";
        public const string IDENTIFIER_ROLE = "DA3B77DF-F495-45C7-9AAC-EC083B99A9D3";
        public const string IDENTIFIER_USER = "2819c223-7f76-453a-919d-413861904646";
        public const string IDENTIFIER_EXTERNAL = "bjensen";

        public const int LIMIT_PAGE_SIZE = 6;

        public const string LOCALE = "en-Us";
        public const string MANAGER_DISPLAY_NAME = "John Smith";
        public const string MANAGER_IDENTIFIER = "26118915-6090-4610-87e4-49d8ca9f808d";
        private const string NAME_FAMILY = "Jensen";
        private const string NAME_FORMATTED = "Ms. Barbara J Jensen III";
        private const string NAME_GIVEN = "Barbara";
        private const string NAME_HONORIFIC_PREFIX = "Ms.";
        private const string NAME_HONORIFIC_SUFFIX = "III";
        private const string NAME_USER = "bjensen";
        public const string PHOTO_VALUE = "https://photos.example.com/profilephoto/72930000000Ccne/F";
        public const string PROFILE_URL = "https://login.example.com/bjensen";
        public const string ROLE_DESCRIPTION = "Attends an educational institution";
        public const string ROLE_DISPLAY = "Student";
        public const string ROLE_VALUE = "student";
        public const string TIME_ZONE = "America/Los_Angeles";
        public const string USER_TYPE = "Employee";

        private readonly ElectronicMailAddress _sampleElectronicMailAddressHome;
        private readonly ElectronicMailAddress _sampleElectronicMailAddressWork;

        private readonly IReadOnlyCollection<ElectronicMailAddress> _sampleElectronicMailAddresses;
        private readonly Manager _sampleManager;
        private readonly Name _sampleName;
        private readonly OperationValue _sampleOperationValue;
        private readonly PatchOperation2Combined _sampleOperation;

        public SampleProvider()
        {
            _sampleElectronicMailAddressHome = new ElectronicMailAddress
            {
                ItemType = ElectronicMailAddressBase.HOME,
                Value = ELECTRONIC_MAIL_ADDRESS_HOME
            };

            _sampleElectronicMailAddressWork = new ElectronicMailAddress
            {
                ItemType = ELECTRONIC_MAIL_ADDRESS_WORK,
                Primary = true,
                Value = ELECTRONIC_MAIL_ADDRESS_WORK
            };

            _sampleElectronicMailAddresses = new ElectronicMailAddress[]
                    {
                        _sampleElectronicMailAddressHome,
                        _sampleElectronicMailAddressWork
                    };

            _sampleManager = new Manager
            {
                Value = MANAGER_IDENTIFIER,
            };

            _sampleName = new Name
            {
                FamilyName = NAME_FAMILY,
                Formatted = NAME_FORMATTED,
                GivenName = NAME_GIVEN,
                HonorificPrefix = NAME_HONORIFIC_PREFIX,
                HonorificSuffix = NAME_HONORIFIC_SUFFIX
            };

            _sampleOperationValue = new OperationValue
            {
                Value = IDENTIFIER_USER
            };

            _sampleOperation = ConstructOperation();

            SamplePatch = ConstructPatch();

            SampleUser = new Core2EnterpriseUser
            {
                Active = true,
                ElectronicMailAddresses = _sampleElectronicMailAddresses,
                ExternalIdentifier = IDENTIFIER_EXTERNAL,
                Identifier = IDENTIFIER_USER,
                Name = _sampleName,
                UserName = NAME_USER,
                EnterpriseExtension = new ExtensionAttributeEnterpriseUser2
                {
                    CostCenter = EXTENSION_ATTRIBUTE_ENTERPRISE_USER_COST_CENTER,
                    Department = EXTENSION_ATTRIBUTE_ENTERPRISE_USER_DEPARTMENT,
                    Division = EXTENSION_ATTRIBUTE_ENTERPRISE_USER_DIVISION,
                    EmployeeNumber = EXTENSION_ATTRIBUTE_ENTERPRISE_USER_EMPLOYEE_NUMBER,
                    Manager = _sampleManager,
                    Organization = EXTENSION_ATTRIBUTE_ENTERPRISE_USER_ORGANIZATION
                }
            };

            SampleGroup = new Core2Group
            {
                DisplayName = GROUP_NAME,
            };
        }

        public Core2Group SampleGroup { get; }

        public PatchRequest2 SamplePatch { get; }

        public Core2EnterpriseUser SampleResource
        {
            get { return SampleUser; }
        }

        public Core2EnterpriseUser SampleUser { get; }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            resource.Identifier = IDENTIFIER_USER;

            return Task.FromResult(resource);
        }

        private PatchOperation2Combined ConstructOperation()
        {
            var path = Path.Create(AttributeNames.MEMBERS);

            return new PatchOperation2Combined
            {
                Name = OperationName.Add,
                Path = path,
                Value = JsonConvert.SerializeObject(_sampleOperationValue)
            };
        }

        private PatchRequest2 ConstructPatch()
        {
            var result = new PatchRequest2();
            result.AddOperation(_sampleOperation);
            return result;
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            return Task.WhenAll();
        }

        private static bool HasMember(IResourceIdentifier containerIdentifier, string memberAttributePath, string memberIdentifier)
        {
            if (containerIdentifier == null)
            {
                throw new ArgumentNullException(nameof(containerIdentifier));
            }

            if (string.IsNullOrWhiteSpace(memberAttributePath))
            {
                throw new ArgumentNullException(nameof(memberAttributePath));
            }

            if (string.IsNullOrWhiteSpace(memberIdentifier))
            {
                throw new ArgumentNullException(nameof(memberIdentifier));
            }

            if (string.IsNullOrWhiteSpace(containerIdentifier.Identifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidIdentifier);
            }

            if (!string.Equals(memberAttributePath, AttributeNames.MEMBERS, StringComparison.Ordinal))
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate,
                    memberAttributePath);

                throw new NotSupportedException(exceptionMessage);
            }

            if (!string.Equals(SchemaIdentifiers.CORE_2_GROUP, containerIdentifier.SchemaIdentifier, StringComparison.Ordinal))
            {
                throw new NotSupportedException(ServiceResources.ExceptionFilterNotSupported);
            }

            return string.Equals(IDENTIFIER_GROUP, containerIdentifier.Identifier, StringComparison.OrdinalIgnoreCase)
                && string.Equals(IDENTIFIER_USER, memberIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        private Resource[] Query(IQueryParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.AlternateFilters.Count != 1)
            {
                throw new NotSupportedException(ServiceResources.ExceptionFilterCount);
            }

            if (parameters.PaginationParameters != null)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionPaginationIsNotSupportedTemplate,
                    parameters.SchemaIdentifier);

                throw new NotSupportedException(exceptionMessage);
            }

            var filter = parameters.AlternateFilters.Single();

            if (filter.AdditionalFilter != null)
            {
                return QueryMember(parameters, filter);
            }
            else if (string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.OrdinalIgnoreCase))
            {
                return QueryUsers(parameters, filter);
            }
            else if (string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.CORE_2_GROUP, StringComparison.OrdinalIgnoreCase))
            {
                return QueryGroups(parameters, filter);
            }
            else
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate,
                    filter.AttributePath);

                throw new NotSupportedException(exceptionMessage);
            }
        }

        public override Task<QueryResponseBase> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            var resources = Query(parameters);

            var result = new QueryResponse(resources.ToList() as IList<Resource>);

            result.TotalResults = result.ItemsPerPage = resources.Length;

            return Task.FromResult(result as QueryResponseBase);
        }

        private Resource[] QueryGroups(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (parameters.ExcludedAttributePaths?.Any() != true
                || parameters.ExcludedAttributePaths.Count != 1
                || !parameters.ExcludedAttributePaths.Single().Equals(AttributeNames.MEMBERS, StringComparison.Ordinal))
            {
                throw new ArgumentException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(filter.AttributePath, AttributeNames.EXTERNAL_IDENTIFIER, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(filter.AttributePath, AttributeNames.DISPLAY_NAME, StringComparison.OrdinalIgnoreCase))
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate,
                    filter.AttributePath);

                throw new NotSupportedException(exceptionMessage);
            }

            if (filter.FilterOperator != ComparisonOperator.Equals)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionFilterOperatorNotSupportedTemplate,
                    filter.FilterOperator);

                throw new NotSupportedException(exceptionMessage);
            }

            return !string.Equals(filter.ComparisonValue, GROUP_NAME, StringComparison.OrdinalIgnoreCase)
                ? Enumerable.Empty<Resource>().ToArray()
                : SampleGroup.ToCollection().ToArray();
        }

        private static Resource[] QueryMember(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (filter.AdditionalFilter == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (parameters.ExcludedAttributePaths?.Any() == true)
            {
                throw new ArgumentException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.CORE_2_GROUP, StringComparison.Ordinal))
            {
                throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (parameters.RequestedAttributePaths?.Any() != true)
            {
                throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (filter.AdditionalFilter.AdditionalFilter != null)
            {
                throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);
            }

            var selectedAttribute = parameters.RequestedAttributePaths.SingleOrDefault();

            if (string.IsNullOrWhiteSpace(selectedAttribute))
            {
                throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(selectedAttribute, AttributeNames.IDENTIFIER, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);
            }

            IReadOnlyCollection<IFilter> filters = new IFilter[]
            {
                filter,
                filter.AdditionalFilter
            };

            var filterIdentifier = filters.SingleOrDefault(
                item => item.AttributePath.Equals(AttributeNames.IDENTIFIER, StringComparison.OrdinalIgnoreCase))
                ?? throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);

            var filterMembers = filters.SingleOrDefault(
                item => item.AttributePath.Equals(AttributeNames.MEMBERS, StringComparison.OrdinalIgnoreCase))
                ?? throw new NotSupportedException(ServiceResources.ExceptionQueryNotSupported);

            var containerIdentifier = new ResourceIdentifier
            {
                SchemaIdentifier = parameters.SchemaIdentifier,
                Identifier = filterIdentifier.ComparisonValue
            };

            if (!HasMember(containerIdentifier, filterMembers.AttributePath, filterMembers.ComparisonValue))
            {
                return Array.Empty<Resource>();
            }
            else
            {
                var container = new Core2Group
                {
                    Identifier = containerIdentifier.Identifier
                };

                return container.ToCollection().ToArray();
            }
        }

        private Resource[] QueryUsers(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (parameters.ExcludedAttributePaths?.Any() == true)
            {
                throw new ArgumentException(ServiceResources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(filter.AttributePath, AttributeNames.EXTERNAL_IDENTIFIER, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(filter.AttributePath, AttributeNames.USER_NAME, StringComparison.OrdinalIgnoreCase))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ServiceResources.ExceptionFilterAttributePathNotSupportedTemplate,
                        filter.AttributePath);
                throw new NotSupportedException(exceptionMessage);
            }

            if (filter.FilterOperator != ComparisonOperator.Equals)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ServiceResources.ExceptionFilterOperatorNotSupportedTemplate,
                        filter.FilterOperator);
                throw new NotSupportedException(exceptionMessage);
            }

            return !string.Equals(filter.ComparisonValue, IDENTIFIER_EXTERNAL, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(filter.ComparisonValue, SampleUser.UserName, StringComparison.OrdinalIgnoreCase)
                ? Enumerable.Empty<Resource>().ToArray()
                : SampleUser.ToCollection().ToArray();
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (resource.Identifier == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidResource);
            }

            if (resource.Is(SchemaIdentifiers.CORE_2_ENTERPRISE_USER)
                && string.Equals(resource.Identifier, IDENTIFIER_USER, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(resource);
            }

            throw new ResourceNotFoundException();
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.ResourceIdentifier == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidParameters);
            }

            Resource resource = null;

            if (string.Equals(parameters.ResourceIdentifier.SchemaIdentifier, SchemaIdentifiers.CORE_2_ENTERPRISE_USER, StringComparison.Ordinal)
                && string.Equals(parameters.ResourceIdentifier.Identifier, IDENTIFIER_USER, StringComparison.OrdinalIgnoreCase))
            {
                resource = SampleUser;
            }

            return Task.FromResult(resource);
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            return Task.WhenAll();
        }
    }
}
