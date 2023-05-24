// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public sealed class RetrievalRequest :
        SCIMRequest<IResourceRetrievalParameters>
    {
        public RetrievalRequest(HttpRequest request, IResourceRetrievalParameters payload,
            string correlationIdentifier, IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
