// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IMetadataProvider
    {
        Core2ServiceConfiguration Configuration { get; }
        IReadOnlyCollection<Core2ResourceType> ResourceTypes { get; }
        IReadOnlyCollection<TypeScheme> Schema { get; }
    }
}
