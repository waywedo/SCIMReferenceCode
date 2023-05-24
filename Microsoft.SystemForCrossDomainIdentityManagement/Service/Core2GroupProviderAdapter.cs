// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM
{
    internal class Core2GroupProviderAdapter : ProviderAdapterTemplate<Core2Group>
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
