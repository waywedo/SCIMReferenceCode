//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Threading;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public abstract class ProtocolJsonDeserializingFactory : ProtocolJsonDeserializingFactory<Schematized>
    {
    }

    public abstract class ProtocolJsonDeserializingFactory<T> : JsonDeserializingFactory<T>
    {
        private IJsonNormalizationBehavior _jsonNormalizer;

        public override IJsonNormalizationBehavior JsonNormalizer
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _jsonNormalizer, () => new ProtocolJsonNormalizer());
            }
        }
    }
}