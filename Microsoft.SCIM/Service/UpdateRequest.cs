// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service.Contracts;
using System.Collections.Generic;

namespace Microsoft.SCIM.Service
{
    public sealed class UpdateRequest : SCIMRequest<IPatch>
    {
        public UpdateRequest(HttpRequest request, IPatch payload, string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions) : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
