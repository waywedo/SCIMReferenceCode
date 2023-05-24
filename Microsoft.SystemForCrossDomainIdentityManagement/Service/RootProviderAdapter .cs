// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.SCIM
{
    internal class RootProviderAdapter : ProviderAdapterTemplate<Resource>
    {
        public RootProviderAdapter(IProvider provider) : base(provider)
        {
        }

        public override string SchemaIdentifier
        {
            get { return SchemaIdentifiers.NONE; }
        }

        public override Task<Resource> Create(HttpRequest request, Resource resource,
            string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override IResourceIdentifier CreateResourceIdentifier(string identifier)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(HttpRequest request, string identifier, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task<Resource> Replace(HttpRequest request, Resource resource, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task<Resource> Retrieve(HttpRequest request, string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task Update(HttpRequest request, string identifier, PatchRequestBase patchRequest,
            string correlationIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
