//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas.Contracts
{
    public interface IChange
    {
        string Watermark { get; set; }
    }
}