// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Controllers;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;

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
