//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Service
{
    using System;
    using Microsoft.SCIM.Protocol;
    using Microsoft.SCIM.Service.Contracts;

    internal class BulkOperationState<TPayload> : BulkOperationStateBase<TPayload> where TPayload : class
    {
        public BulkOperationState(
            IRequest<BulkRequest2> request,
            BulkRequestOperation operation,
            IBulkOperationContext<TPayload> context)
            : base(request, operation, context)
        {
        }

        public override bool TryPrepareRequest(out IRequest<TPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
