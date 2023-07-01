// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Controllers;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;
using System;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_SERVICE_CONFIGURATION)]
    //[Authorize]
    [ApiController]
    public sealed class ServiceProviderConfigurationController : ControllerTemplate
    {
        private readonly IProvider _provider;
        private readonly ILogger<ServiceProviderConfigurationController> _logger;

        public ServiceProviderConfigurationController(IProvider provider, ILogger<ServiceProviderConfigurationController> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<ServiceConfigurationBase> Get()
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

                return provider.Configuration;
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Service provider configuration controller get", requestId);

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Service provider configuration controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Service provider configuration controller get", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Service provider configuration controller get", requestId);

                throw;
            }
        }
    }
}
