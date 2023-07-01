//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net.Http;

namespace Microsoft.SCIM.Protocol.Contracts
{
    public interface IExtension
    {
        Type Controller { get; }
        JsonDeserializingFactory JsonDeserializingFactory { get; }
        string Path { get; }
        string SchemaIdentifier { get; }
        string TypeName { get; }

        bool Supports(HttpRequestMessage request);
    }
}