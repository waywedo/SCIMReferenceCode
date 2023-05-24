// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;

namespace Microsoft.SCIM
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
