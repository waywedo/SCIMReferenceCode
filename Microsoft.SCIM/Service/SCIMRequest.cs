// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public abstract class SCIMRequest<TPayload> : IRequest<TPayload>
        where TPayload : class
    {
        protected SCIMRequest(HttpRequest request, TPayload payload,
            string correlationIdentifier, IReadOnlyCollection<IExtension> extensions)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            BaseResourceIdentifier = request.GetBaseResourceIdentifier();
            Request = request;
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            CorrelationIdentifier = correlationIdentifier;
            Extensions = extensions;
        }

        public Uri BaseResourceIdentifier { get; }

        public string CorrelationIdentifier { get; }

        public IReadOnlyCollection<IExtension> Extensions { get; }

        public TPayload Payload { get; }

        public HttpRequest Request { get; }
    }
}
