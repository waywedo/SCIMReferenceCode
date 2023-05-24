//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net.Http;

namespace Microsoft.SCIM
{
    internal class InvalidBulkOperationContext : IBulkOperationContext
    {
        private readonly IBulkOperationState _state;

        public InvalidBulkOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _state = new InvalidBulkOperationState(request, operation);
        }

        public bool Completed => true;

        public bool Faulted => true;

        public IRequest<BulkRequest2> BulkRequest => _state.BulkRequest;

        public HttpMethod Method => _state.Operation.Method;

        public BulkRequestOperation Operation => _state.Operation;

        public BulkResponseOperation Response => _state.Response;

        public void Complete(BulkResponseOperation response) => _state.Complete(response);

        public bool TryPrepare() => _state.TryPrepare();
    }
}
