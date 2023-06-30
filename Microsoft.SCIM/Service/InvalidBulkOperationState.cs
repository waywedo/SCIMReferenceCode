//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;

namespace Microsoft.SCIM
{
    internal class InvalidBulkOperationState : IBulkOperationState
    {
        public InvalidBulkOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            BulkRequest = request ?? throw new ArgumentNullException(nameof(request));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        public IRequest<BulkRequest2> BulkRequest { get;  }

        public BulkRequestOperation Operation { get;  }

        public BulkResponseOperation Response { get; private set; }

        public void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Response is ErrorResponse)
            {
                Response = response;
            }
            else
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidResponse);
            }
        }

        public bool TryPrepare() => false;
    }
}
