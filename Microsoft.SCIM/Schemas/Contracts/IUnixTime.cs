//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;

namespace Microsoft.SCIM.Schemas.Contracts
{
    public interface IUnixTime
    {
        long EpochTimestamp { get; }

        DateTime ToUniversalTime();
    }
}