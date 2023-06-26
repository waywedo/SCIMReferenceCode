// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.SCIM
{
    public sealed class RootController : ControllerTemplate<Resource>
    {
        public RootController(IProvider provider, ILogger<RootController> logger)
            : base(new RootProviderAdapter(provider), logger)
        {
        }
    }
}
