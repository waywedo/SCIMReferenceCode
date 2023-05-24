// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_RESOURCE_TYPES)]
    //[Authorize]
    [ApiController]
    public sealed class ResourceTypesController : ControllerTemplate
    {
        private readonly IProvider _provider;
        private readonly ILogger<ResourceTypesController> _logger;

        public ResourceTypesController(IProvider provider, ILogger<ResourceTypesController> logger) : base()
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<QueryResponseBase> Get()
        {
            string requestId = null;

            try
            {
                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var provider = _provider;

                if (provider == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var resources = provider.ResourceTypes;
                var result = new QueryResponse(resources);

                result.TotalResults = result.ItemsPerPage = resources.Count;
                result.StartIndex = 1;

                return result;
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Resource types controller get", requestId);

                return StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Resource types controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Resource types controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Resource types controller get", requestId);

                throw;
            }
        }
    }
}
