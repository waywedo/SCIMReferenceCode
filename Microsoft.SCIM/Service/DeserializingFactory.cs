// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;

namespace Microsoft.SCIM
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
