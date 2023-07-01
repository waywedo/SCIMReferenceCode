// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.Protocol.Contracts;
using System.Collections.Generic;

namespace Microsoft.SCIM.Service
{
    public sealed class DeletionRequest : SCIMRequest<IResourceIdentifier>
    {
        public DeletionRequest(HttpRequest request, IResourceIdentifier payload, string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
