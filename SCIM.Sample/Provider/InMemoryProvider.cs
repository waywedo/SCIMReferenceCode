// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SCIM;
using SCIM.Sample.Resources;

namespace SCIM.Sample.Provider
{
    public class InMemoryProvider : ProviderBase
    {
        private readonly ProviderBase _groupProvider;
        private readonly ProviderBase _userProvider;

        private static readonly Lazy<IReadOnlyCollection<TypeScheme>> TypeSchema =
            new(
                () =>
                    new TypeScheme[]
                    {
                        SampleTypeScheme.UserTypeScheme,
                        SampleTypeScheme.GroupTypeScheme,
                        SampleTypeScheme.EnterpriseUserTypeScheme,
                        SampleTypeScheme.ResourceTypesTypeScheme,
                        SampleTypeScheme.SchemaTypeScheme,
                        SampleTypeScheme.ServiceProviderConfigTypeScheme
                    });

        private static readonly Lazy<IReadOnlyCollection<Core2ResourceType>> Types =
            new(
                () =>
                    new Core2ResourceType[] { SampleResourceTypes.UserResourceType, SampleResourceTypes.GroupResourceType });

        public InMemoryProvider()
        {
            _groupProvider = new InMemoryGroupProvider();
            _userProvider = new InMemoryUserProvider();
        }

        public override IReadOnlyCollection<Core2ResourceType> ResourceTypes => Types.Value;

        public override IReadOnlyCollection<TypeScheme> Schema => TypeSchema.Value;

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource is Core2EnterpriseUser)
            {
                return _userProvider.CreateAsync(resource, correlationIdentifier);
            }

            if (resource is Core2Group)
            {
                return _groupProvider.CreateAsync(resource, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (resourceIdentifier.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER))
            {
                return _userProvider.DeleteAsync(resourceIdentifier, correlationIdentifier);
            }

            if (resourceIdentifier.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_GROUP))
            {
                return _groupProvider.DeleteAsync(resourceIdentifier, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task<QueryResponseBase> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER))
            {
                return _userProvider.QueryAsync(parameters, correlationIdentifier);
            }

            if (parameters.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_GROUP))
            {
                return _groupProvider.QueryAsync(parameters, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource is Core2EnterpriseUser)
            {
                return _userProvider.ReplaceAsync(resource, correlationIdentifier);
            }

            if (resource is Core2Group)
            {
                return _groupProvider.ReplaceAsync(resource, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER))
            {
                return _userProvider.RetrieveAsync(parameters, correlationIdentifier);
            }

            if (parameters.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_GROUP))
            {
                return _groupProvider.RetrieveAsync(parameters, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.SchemaIdentifier))
            {
                throw new ArgumentException(nameof(patch));
            }

            if (patch.ResourceIdentifier.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_ENTERPRISE_USER))
            {
                return _userProvider.UpdateAsync(patch, correlationIdentifier);
            }

            if (patch.ResourceIdentifier.SchemaIdentifier.Equals(SchemaIdentifiers.CORE_2_GROUP))
            {
                return _groupProvider.UpdateAsync(patch, correlationIdentifier);
            }

            throw new NotImplementedException();
        }
    }
}
