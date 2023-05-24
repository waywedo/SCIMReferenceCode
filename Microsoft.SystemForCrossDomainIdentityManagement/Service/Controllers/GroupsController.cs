// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_GROUPS)]
    //[Authorize]
    [ApiController]
    public sealed class GroupsController : ControllerTemplate<Core2Group>
    {
        public GroupsController(IProvider provider, ILogger<GroupsController> logger)
            : base(new Core2GroupProviderAdapter(provider), logger)
        {
        }
    }
}
