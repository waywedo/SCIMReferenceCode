//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    public static class ProviderExtension
    {
        public static IReadOnlyCollection<IExtension> ReadExtensions(this IProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IReadOnlyCollection<IExtension> result;

            try
            {
                result = provider.Extensions;
            }
            catch (NotImplementedException)
            {
                result = null;
            }

            return result;
        }
    }
}
