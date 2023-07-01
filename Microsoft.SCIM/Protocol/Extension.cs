//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net.Http;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public abstract class Extension : IExtension
    {
        protected Extension(string schemaIdentifier, string typeName, string path, Type controller)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            SchemaIdentifier = schemaIdentifier;
            TypeName = typeName;
            Path = path;
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public Type Controller { get; }

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