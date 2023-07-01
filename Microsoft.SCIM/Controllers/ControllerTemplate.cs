// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Controllers
{
    public abstract class ControllerTemplate : ControllerBase
    {
        internal const string ATTRIBUTE_VALUE_IDENTIFIER = "{identifier}";
        private const string HEADER_KEY_CONTENT_TYPE = "Content-Type";
        private const string HEADER_KEY_LOCATION = "Location";

        protected ControllerTemplate()
        {
        }

        protected virtual void ConfigureResponse(Resource resource)
        {
            Response.ContentType = ProtocolConstants.CONTENT_TYPE;
            Response.StatusCode = (int)HttpStatusCode.Created;

            if (Response.Headers == null)
            {
                return;
            }

            if (!Response.Headers.ContainsKey(HEADER_KEY_CONTENT_TYPE))
            {
                Response.Headers.Add(HEADER_KEY_CONTENT_TYPE, ProtocolConstants.CONTENT_TYPE);
            }

            var baseResourceIdentifier = Request.GetBaseResourceIdentifier();
            var resourceIdentifier = resource.GetResourceIdentifier(baseResourceIdentifier);
            var resourceLocation = resourceIdentifier.AbsoluteUri;

            if (!Response.Headers.ContainsKey(HEADER_KEY_LOCATION))
            {
                Response.Headers.Add(HEADER_KEY_LOCATION, resourceLocation);
            }
        }

        protected ObjectResult ScimError(HttpStatusCode httpStatusCode, string message)
        {
            return StatusCode((int)httpStatusCode, new Core2Error(message, (int)httpStatusCode));
        }
    }

    public abstract class ControllerTemplate<T> : ControllerTemplate where T : Resource
    {
        private readonly IProviderAdapter<T> _provider;
        private readonly ILogger _logger;

        protected ControllerTemplate(IProviderAdapter<T> provider, ILogger logger)
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpDelete(ATTRIBUTE_VALUE_IDENTIFIER)]
        public virtual async Task<IActionResult> Delete(string identifier)
        {
            string requestId = null;

            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return BadRequest();
                }

                identifier = Uri.UnescapeDataString(identifier);

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                await _provider.Delete(Request, identifier, requestId).ConfigureAwait(false);

                return NoContent();
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template delete", requestId);

                return BadRequest();
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template delete", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template delete", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template delete", requestId);

                throw;
            }
        }

        [HttpGet]
        public virtual async Task<ActionResult<QueryResponseBase>> Get()
        {
            string requestId = null;

            try
            {
                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var resourceQuery = new ResourceQuery(Request.Query);

                var result = await _provider.Query(Request, resourceQuery.Filters, resourceQuery.Attributes,
                    resourceQuery.ExcludedAttributes, resourceQuery.PaginationParameters, requestId)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template get", requestId);

                return ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template get", requestId);

                return ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template get", requestId);

                return ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template get", requestId);

                return ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        [HttpGet(ATTRIBUTE_VALUE_IDENTIFIER)]
        public virtual async Task<IActionResult> Get(string identifier)
        {
            string requestId = null;

            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return ScimError(HttpStatusCode.BadRequest, ServiceResources.ExceptionInvalidIdentifier);
                }

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var resourceQuery = new ResourceQuery(Request.Query);

                if (resourceQuery.Filters.Any())
                {
                    if (resourceQuery.Filters.Count != 1)
                    {
                        return ScimError(HttpStatusCode.BadRequest, ServiceResources.ExceptionFilterCount);
                    }

                    var filter = new Filter(AttributeNames.IDENTIFIER, ComparisonOperator.Equals, identifier)
                    {
                        AdditionalFilter = resourceQuery.Filters.Single()
                    };

                    var filters = new IFilter[] { filter };

                    var effectiveQuery = new ResourceQuery(filters, resourceQuery.Attributes,
                        resourceQuery.ExcludedAttributes);

                    var queryResponse = await _provider.Query(Request, effectiveQuery.Filters, effectiveQuery.Attributes,
                        effectiveQuery.ExcludedAttributes, effectiveQuery.PaginationParameters, requestId)
                        .ConfigureAwait(false);

                    if (!queryResponse.Resources.Any())
                    {
                        return ScimError(HttpStatusCode.NotFound,
                            string.Format(ServiceResources.ResourceNotFoundTemplate,
                            identifier));
                    }

                    var result = queryResponse.Resources.Single();

                    return Ok(result);
                }
                else
                {
                    var result = await _provider.Retrieve(Request, identifier, resourceQuery.Attributes,
                        resourceQuery.ExcludedAttributes, requestId).ConfigureAwait(false);

                    if (result == null)
                    {
                        return ScimError(HttpStatusCode.NotFound,
                            string.Format(ServiceResources.ResourceNotFoundTemplate,
                            identifier));
                    }

                    return Ok(result);
                }
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template get by id", requestId);

                return ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template get by id", requestId);

                return ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template get by id", requestId);

                return ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (ResourceNotFoundException)
            {
                return ScimError(HttpStatusCode.NotFound, string.Format(ServiceResources.ResourceNotFoundTemplate, identifier));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template get by id", requestId);

                return ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        [HttpPatch(ATTRIBUTE_VALUE_IDENTIFIER)]
        public virtual async Task<IActionResult> Patch(string identifier, [FromBody] PatchRequest2 patchRequest)
        {
            string requestId = null;

            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return BadRequest();
                }

                identifier = Uri.UnescapeDataString(identifier);

                if (patchRequest == null)
                {
                    return BadRequest();
                }

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                await _provider.Update(Request, identifier, patchRequest, requestId)
                    .ConfigureAwait(false);

                // If EnterpriseUser, return HTTP code 200 and user object, otherwise HTTP code 204
                if (_provider.SchemaIdentifier == SchemaIdentifiers.CORE_2_ENTERPRISE_USER)
                {
                    return await Get(identifier).ConfigureAwait(false);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template patch", requestId);

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template patch", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template patch", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template patch", requestId);

                throw;
            }
        }

        [HttpPost]
        public virtual async Task<ActionResult<Resource>> Post([FromBody] T resource)
        {
            string requestId = null;

            try
            {
                if (resource == null)
                {
                    return BadRequest();
                }

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var result = await _provider.Create(Request, resource, requestId).ConfigureAwait(false);

                ConfigureResponse(result);

                return CreatedAtAction(nameof(Post), result);
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template post", requestId);

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template post", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template post", requestId);

                return StatusCode(StatusCodes.Status501NotImplemented);
            }
            catch (ResourceConflictException ex)
            {
                _logger.LogError(ex, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template post", requestId);

                throw;
            }
        }

        [HttpPut(ATTRIBUTE_VALUE_IDENTIFIER)]
        public virtual async Task<ActionResult<Resource>> Put([FromBody] T resource, string identifier)
        {
            string requestId = null;

            try
            {
                if (resource == null)
                {
                    return ScimError(HttpStatusCode.BadRequest, ServiceResources.ExceptionInvalidResource);
                }

                if (string.IsNullOrEmpty(identifier))
                {
                    return ScimError(HttpStatusCode.BadRequest, ServiceResources.ExceptionInvalidIdentifier);
                }

                if (!Request.TryGetRequestIdentifier(out requestId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                resource.Identifier = identifier;

                var result = await _provider.Replace(Request, resource, requestId).ConfigureAwait(false);

                ConfigureResponse(result);

                return Ok(result);
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                _logger.LogError(notImplementedException, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                _logger.LogError(notSupportedException, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.NotFound, string.Format(ServiceResources.ResourceNotFoundTemplate, identifier));
            }
            catch (ResourceConflictException ex)
            {
                _logger.LogError(ex, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.Conflict, ServiceResources.ExceptionInvalidRequest);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{requestId} Controller template put", requestId);

                return ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}
