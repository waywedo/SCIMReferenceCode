using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service
{
    public class SchematizedTextInputFormatter : TextInputFormatter
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        private static readonly Lazy<MediaTypeHeaderValue> MediaTypeHeaderJavaWebToken
            = new(() => new MediaTypeHeaderValue(MediaTypes.JAVA_WEB_TOKEN));

        private static readonly Lazy<MediaTypeHeaderValue> MediaTypeHeaderJson =
            new(() => new MediaTypeHeaderValue(MediaTypes.JSON));

        private static readonly Lazy<MediaTypeHeaderValue> MediaTypeHeaderProtocol =
            new(() => new MediaTypeHeaderValue(MediaTypes.PROTOCOL));

        private static readonly Lazy<MediaTypeHeaderValue> MediaTypeHeaderStream =
            new(() => new MediaTypeHeaderValue(MediaTypes.STREAM));
        private readonly JsonDeserializingFactory<Schematized> _deserializingFactory;
        private readonly ILogger<SchematizedTextInputFormatter> _logger;

        public SchematizedTextInputFormatter(JsonDeserializingFactory<Schematized> deserializingFactory, ILogger<SchematizedTextInputFormatter> logger)
        {
            ArgumentNullException.ThrowIfNull(deserializingFactory, nameof(deserializingFactory));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _deserializingFactory = deserializingFactory;
            _logger = logger;

            SupportedMediaTypes.Add(MediaTypeHeaderJavaWebToken.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderJson.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderProtocol.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderStream.Value);
        }

        private static bool CanProcessType(Type type)
        {
            var schematizedType = typeof(Schematized);
            return schematizedType.IsAssignableFrom(type) || type == typeof(string);
        }

        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return CanProcessType(type);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var type = context.ModelType;

            if (!typeof(IDictionary<string, object>).IsAssignableFrom(type)
                && !typeof(Schematized).IsAssignableFrom(type)
                && typeof(string) != type)
            {
                throw new NotSupportedException(type.FullName);
            }

            using var reader = new StreamReader(context.HttpContext.Response.Body, Encoding);

            var characters = await reader.ReadToEndAsync().ConfigureAwait(false);

            _logger.LogInformation($"{ServiceResources.InformationRead}{{characters}}", characters);

            if (string.Equals(context.HttpContext.Request.Headers.ContentType, MediaTypes.JAVA_WEB_TOKEN, StringComparison.Ordinal))
            {
                return await InputFormatterResult.SuccessAsync(characters);
            }

            var json = JsonFactory.Instance.Create(characters, _deserializingFactory.AcceptLargeObjects);

            if (typeof(IDictionary<string, object>).IsAssignableFrom(type))
            {
                return await InputFormatterResult.SuccessAsync(json);
            }

            try
            {
                return await InputFormatterResult.SuccessAsync(_deserializingFactory.Create(json));
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}