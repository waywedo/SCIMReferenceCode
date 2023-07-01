// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service
{
    public sealed class SchematizedTextOutputFormatter : TextOutputFormatter
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
        private readonly ILogger<SchematizedTextOutputFormatter> _logger;

        public SchematizedTextOutputFormatter(JsonDeserializingFactory<Schematized> deserializingFactory,
            ILogger<SchematizedTextOutputFormatter> logger)
        {
            ArgumentNullException.ThrowIfNull(deserializingFactory, nameof(deserializingFactory));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            SupportedMediaTypes.Add(MediaTypeHeaderJavaWebToken.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderJson.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderProtocol.Value);
            SupportedMediaTypes.Add(MediaTypeHeaderStream.Value);

            _deserializingFactory = deserializingFactory;
            _logger = logger;
        }

        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof(Schematized).IsAssignableFrom(type) || type == typeof(string);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.ObjectType == null)
            {
                throw new ArgumentNullException(nameof(context.ObjectType));
            }

            if (context.Object == null)
            {
                throw new ArgumentNullException(nameof(context.Object));
            }

            string characters;

            if (typeof(string) == context.ObjectType)
            {
                characters = (string)context.Object;
            }
            else
            {
                IDictionary<string, object> json;

                if (typeof(IDictionary<string, object>).IsAssignableFrom(context.ObjectType))
                {
                    json = (IDictionary<string, object>)context.Object;
                    characters = JsonFactory.Instance.Create(json, _deserializingFactory.AcceptLargeObjects);
                }
                else if (typeof(Schematized).IsAssignableFrom(context.ObjectType))
                {
                    Schematized schematized = (Schematized)context.Object;
                    json = schematized.ToJson();
                    characters = JsonFactory.Instance.Create(json, _deserializingFactory.AcceptLargeObjects);
                }
                else
                {
                    throw new NotSupportedException(context.ObjectType.FullName);
                }
            }

            _logger.LogInformation($"{ServiceResources.InformationWrote}{{characters}}", characters);

            return context.HttpContext.Response.WriteAsync(string.Concat(ServiceResources.InformationWrote,
                    characters), Encoding);
        }
    }
}
