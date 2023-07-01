//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Net;

namespace Microsoft.SCIM.Protocol.Contracts
{
    internal interface IResponse
    {
        HttpStatusCode Status { get; set; }
        string StatusCodeValue { get; set; }

        bool IsError();
    }
}