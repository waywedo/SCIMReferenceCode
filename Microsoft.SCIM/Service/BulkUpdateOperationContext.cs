//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    internal sealed class BulkUpdateOperationContext : BulkOperationContextBase<IPatch>, IBulkUpdateOperationContext
    {
        private readonly IBulkUpdateOperationState _receivedState;

        public BulkUpdateOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _receivedState = new BulkUpdateOperationState(request, operation, this);

            Initialize(_receivedState);
        }

        public BulkUpdateOperationContext( IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkCreationOperationContext parent)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _receivedState = new BulkUpdateOperationState(request, operation, this, parent);

            Initialize(_receivedState);
        }

        public IReadOnlyCollection<IBulkCreationOperationContext> Dependencies => _receivedState.Dependencies;

        public IBulkCreationOperationContext Parent => _receivedState.Parent;

        public void AddDependency(IBulkCreationOperationContext dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            _receivedState.AddDependency(dependency);
        }
    }
}
