//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using System.Collections.Generic;

namespace Microsoft.SCIM.Service
{
    public sealed class BulkRequest : SCIMRequest<BulkRequest2>
    {
        public BulkRequest(HttpRequest request, BulkRequest2 payload, string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
