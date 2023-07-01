//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class Core2ServiceConfiguration : ServiceConfigurationBase
    {
        public Core2ServiceConfiguration(BulkRequestsFeature bulkRequestsSupport, bool supportsEntityTags,
            bool supportsFiltering, bool supportsPasswordChange, bool supportsPatching, bool supportsSorting)
        {
            AddSchema(SchemaIdentifiers.CORE_2_SERVICE_CONFIGURATION);
            Metadata = new Core2Metadata()
            {
                ResourceType = Types.SERVICE_PROVIDER_CONFIGURATION
            };

            BulkRequests = bulkRequestsSupport;
            EntityTags = new Feature(supportsEntityTags);
            Filtering = new Feature(supportsFiltering);
            PasswordChange = new Feature(supportsPasswordChange);
            Patching = new Feature(supportsPatching);
            Sorting = new Feature(supportsSorting);
        }

        [DataMember(Name = AttributeNames.METADATA)]
        public Core2Metadata Metadata { get; set; }
    }
}