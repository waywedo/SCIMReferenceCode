// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Controllers;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;
using System;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_USERS)]
    //[Authorize]
    [ApiController]
    public sealed class UsersController : ControllerTemplate<Core2EnterpriseUser>
    {
        public UsersController(IProvider provider, ILogger<UsersController> logger)
            : base(new Core2EnterpriseUserProviderAdapter(provider), logger)
        {
        }
    }
}
