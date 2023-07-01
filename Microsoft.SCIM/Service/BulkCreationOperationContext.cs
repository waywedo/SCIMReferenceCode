//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    internal sealed class BulkCreationOperationContext : BulkOperationContextBase<Resource>, IBulkCreationOperationContext
    {
        private readonly IBulkCreationOperationState _receivedState;

        public BulkCreationOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _receivedState = new BulkCreationOperationState(request, operation, this);
            Initialize(_receivedState);
            PendingState = new BulkOperationState<Resource>(request, operation, this);
        }

        public IReadOnlyCollection<IBulkUpdateOperationContext> Dependents => _receivedState.Dependents;

        public IBulkOperationState<Resource> PendingState { get; }

        public IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates => _receivedState.Subordinates;

        public void AddDependent(IBulkUpdateOperationContext dependent)
        {
            if (dependent == null)
            {
                throw new ArgumentNullException(nameof(dependent));
            }

            _receivedState.AddDependent(dependent);
        }

        public void AddSubordinate(IBulkUpdateOperationContext subordinate)
        {
            if (subordinate == null)
            {
                throw new ArgumentNullException(nameof(subordinate));
            }

            _receivedState.AddSubordinate(subordinate);
        }
    }
}
