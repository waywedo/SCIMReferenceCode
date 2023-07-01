// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Service
{
    public abstract class DeserializingFactory<TResource> : JsonDeserializingFactory<TResource>, IResourceJsonDeserializingFactory<TResource>
        where TResource : Resource, new()
    {
        public new TResource Create(IReadOnlyDictionary<string, object> json)
        {
            return base.Create(json);
        }
    }
}
