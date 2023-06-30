//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract(Name = DATA_CONTRACT_NAME)]
    public sealed class Core2EnterpriseUser : Core2EnterpriseUserBase
    {
        private const string DATA_CONTRACT_NAME = "Core2EnterpriseUser";
    }
}