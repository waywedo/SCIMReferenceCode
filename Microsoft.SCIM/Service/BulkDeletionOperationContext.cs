//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    internal sealed class BulkDeletionOperationContext : BulkOperationContextBase<IResourceIdentifier>
    {
        public BulkDeletionOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var receivedState = new BulkDeletionOperationState(request, operation, this);

            Initialize(receivedState);
        }
    }
}
