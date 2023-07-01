// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    public class Core2GroupProviderAdapter : ProviderAdapterTemplate<Core2Group>
    {
        public Core2GroupProviderAdapter(IProvider provider) : base(provider)
        {
        }

        public override string SchemaIdentifier
        {
            get { return SchemaIdentifiers.CORE_2_GROUP; }
        }
    }
}
