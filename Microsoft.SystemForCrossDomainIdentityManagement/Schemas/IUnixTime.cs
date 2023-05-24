//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;

namespace Microsoft.SCIM
{
    public interface IUnixTime
    {
        long EpochTimestamp { get; }

        DateTime ToUniversalTime();
    }
}