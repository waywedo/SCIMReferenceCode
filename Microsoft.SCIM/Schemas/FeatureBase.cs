﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class FeatureBase
    {
        [DataMember(Name = AttributeNames.SUPPORTED)]
        public bool Supported { get; set; }
    }
}