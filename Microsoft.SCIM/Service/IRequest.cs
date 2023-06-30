// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IRequest
    {
        Uri BaseResourceIdentifier { get; }
        string CorrelationIdentifier { get; }
        IReadOnlyCollection<IExtension> Extensions { get; }
        HttpRequest Request { get; }
    }

    public interface IRequest<TPayload> : IRequest where TPayload : class
    {
        TPayload Payload { get; }
    }
}
