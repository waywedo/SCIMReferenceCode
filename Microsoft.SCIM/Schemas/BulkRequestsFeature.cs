//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class BulkRequestsFeature : FeatureBase
    {
        private BulkRequestsFeature()
        {
        }

        public int ConcurrentOperations { get; }

        [DataMember(Name = AttributeNames.MAXIMUM_OPERATIONS)]
        public int MaximumOperations { get; }

        [DataMember(Name = AttributeNames.MAXIMUM_PAYLOAD_SIZE)]
        public int MaximumPayloadSize { get; }

        public static BulkRequestsFeature CreateUnsupportedFeature()
        {
            return new BulkRequestsFeature()
            {
                Supported = false
            };
        }
    }
}