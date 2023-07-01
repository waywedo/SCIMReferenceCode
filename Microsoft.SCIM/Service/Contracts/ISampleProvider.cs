// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface ISampleProvider
    {
        Core2Group SampleGroup { get; }
        PatchRequest2 SamplePatch { get; }
        Core2EnterpriseUser SampleResource { get; }
        Core2EnterpriseUser SampleUser { get; }
    }
}
