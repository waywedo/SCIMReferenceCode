// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IUniformResourceIdentifier
    {
        bool IsQuery { get; }

        IResourceIdentifier Identifier { get; }
        IResourceQuery Query { get; }
    }
}
