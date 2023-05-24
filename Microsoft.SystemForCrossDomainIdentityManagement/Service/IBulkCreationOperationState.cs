//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IBulkCreationOperationState : IBulkOperationState<Resource>
    {
        IReadOnlyCollection<IBulkUpdateOperationContext> Dependents { get; }
        IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates { get; }

        void AddDependent(IBulkUpdateOperationContext dependent);
        void AddSubordinate(IBulkUpdateOperationContext subordinate);
    }
}
