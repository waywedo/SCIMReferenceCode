// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Controllers;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;
using System;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_SCHEMAS)]
    //[Authorize]
    [ApiController]
    public sealed class SchemasController : ControllerTemplate
    {
        private readonly IProvider _provider;
        private readonly ILogger<SchemasController> _logger;

        public SchemasController(IProvider provider, ILogger<SchemasController> logger)
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

                IProvider provider = _provider;

                if (provider == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var resources = provider.Schema;
                var result = new QueryResponse(resources);

                result.TotalResults = result.ItemsPerPage = resources.Count;

                result.StartIndex = 1;

                return result;
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Schemas controller get", requestId);

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Schemas controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Schemas controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Schemas controller get", requestId);

                throw;
            }
        }
    }
}
