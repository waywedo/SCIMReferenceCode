// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Schemas;
using System.Collections.Generic;

namespace Microsoft.SCIM.Service
{
    public sealed class ReplaceRequest : SCIMRequest<Resource>
    {
        public ReplaceRequest(HttpRequest request, Resource payload, string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
