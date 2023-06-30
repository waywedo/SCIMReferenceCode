//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM
{
    public interface IGroupDeserializer
    {
        IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }
    }
}
