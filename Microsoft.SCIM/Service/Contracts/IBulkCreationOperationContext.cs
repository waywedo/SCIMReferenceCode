//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IBulkCreationOperationContext : IBulkOperationContext<Resource>, IBulkCreationOperationState
    {
        IBulkOperationState<Resource> PendingState { get; }
    }
}
