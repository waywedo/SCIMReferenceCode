// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IPatch
    {
        PatchRequestBase PatchRequest { get; set; }
        IResourceIdentifier ResourceIdentifier { get; set; }
    }
}
