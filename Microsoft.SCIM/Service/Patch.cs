// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    public sealed class Patch : IPatch
    {
        public Patch()
        {
        }

        public Patch(IResourceIdentifier resourceIdentifier, PatchRequestBase request)
        {
            ResourceIdentifier = resourceIdentifier ?? throw new ArgumentNullException(nameof(resourceIdentifier));
            PatchRequest = request ?? throw new ArgumentNullException(nameof(request));
        }

        public PatchRequestBase PatchRequest { get; set; }

        public IResourceIdentifier ResourceIdentifier { get; set; }
    }
}
