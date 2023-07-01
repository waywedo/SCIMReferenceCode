//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service.Contracts
{
    public interface IBulkCreationOperationState : IBulkOperationState<Resource>
    {
        IReadOnlyCollection<IBulkUpdateOperationContext> Dependents { get; }
        IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates { get; }

        void AddDependent(IBulkUpdateOperationContext dependent);
        void AddSubordinate(IBulkUpdateOperationContext subordinate);
    }
}
