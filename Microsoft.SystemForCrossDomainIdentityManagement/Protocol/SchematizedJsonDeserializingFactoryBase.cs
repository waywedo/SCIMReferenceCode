﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public abstract class SchematizedJsonDeserializingFactoryBase :
        ProtocolJsonDeserializingFactory, ISchematizedJsonDeserializingFactory
    {
        public abstract IReadOnlyCollection<IExtension> Extensions { get; set; }
        public abstract IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }
        public abstract ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior { get; set; }
        public abstract IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; set; }
    }
}