//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microsoft.SCIM
{
    [Route(ServiceConstants.ROUTE_BULK)]
    //[Authorize]
    [ApiController]
    public sealed class BulkRequestController : ControllerTemplate
    {
        private readonly IProvider _provider;
        private readonly ILogger<BulkRequestController> _logger;

        public BulkRequestController(IProvider provider, ILogger<BulkRequestController> logger) : base()
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BulkRequest2 bulkRequest)
        {
            string requestId = null;

            try
            {
                if (bulkRequest == null)
                {
                    return BadRequest();
                }

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var provider = _provider;

                if (provider == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var extensions = provider.ReadExtensions();
                var response = new BulkRequest(Request, bulkRequest, requestId, extensions);

                return Ok(await provider.ProcessAsync(response).ConfigureAwait(false));
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Bulk request controller post", requestId);

                return BadRequest();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogError(invalidOperationException, "{requestId} Bulk request controller post", requestId);

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Bulk request controller post", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Bulk request controller post", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Bulk request controller post", requestId);

                throw;
            }
        }
    }
}
