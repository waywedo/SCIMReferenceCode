//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.SCIM.EventToken.Schemas;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service;

namespace Microsoft.SCIM.EventToken.Service
{
    public sealed class EventRequest : SCIMRequest<IEventToken>
    {
        public EventRequest(HttpRequest request, IEventToken payload, string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}