//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net.Http;

namespace Microsoft.SCIM
{
    public abstract class Extension : IExtension
    {
        private const string ARGUMENT_NAME_CONTROLLER = "controller";
        private const string ARGUMENT_NAME_JSON_DESERIALIZING_FACTORY = "jsonDeserializingFactory";
        private const string ARGUMENT_NAME_PATH = "path";
        private const string ARGUMENT_NAME_SCHEMA_IDENTIFIER = "schemaIdentifier";
        private const string ARGUMENT_NAME_TYPE_NAME = "typeName";

        protected Extension(string schemaIdentifier, string typeName, string path, Type controller, JsonDeserializingFactory jsonDeserializingFactory)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_SCHEMA_IDENTIFIER);
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_TYPE_NAME);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(ARGUMENT_NAME_PATH);
            }

            SchemaIdentifier = schemaIdentifier;
            TypeName = typeName;
            Path = path;
            Controller = controller ?? throw new ArgumentNullException(ARGUMENT_NAME_CONTROLLER);
            JsonDeserializingFactory = jsonDeserializingFactory ?? throw new ArgumentNullException(ARGUMENT_NAME_JSON_DESERIALIZING_FACTORY);
        }

        public Type Controller { get; }

        public JsonDeserializingFactory JsonDeserializingFactory { get; }

        public string Path { get; }

        public string SchemaIdentifier { get; }

        public string TypeName { get; }

        public virtual bool Supports(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.RequestUri?.AbsolutePath?.EndsWith(Path, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}