//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Protocol.Contracts;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    internal class BulkDeletionOperationState : BulkOperationStateBase<IResourceIdentifier>
    {
        public BulkDeletionOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkOperationContext<IResourceIdentifier> context) : base(request, operation, context)
        {
        }

        public override bool TryPrepareRequest(out IRequest<IResourceIdentifier> request)
        {
            request = null;

            var absoluteResourceIdentifier = new Uri(BulkRequest.BaseResourceIdentifier, Operation.Path);

            if (!UniformResourceIdentifier.TryParse(absoluteResourceIdentifier, BulkRequest.Extensions,
                out IUniformResourceIdentifier resourceIdentifier))
            {
                Context.State = this;

                var error = new ErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    ErrorType = ErrorType.invalidPath
                };
                var response = new BulkResponseOperation(Operation.Identifier)
                {
                    Response = error,
                    Method = Operation.Method,
                    Status = HttpStatusCode.BadRequest
                };

                response.Method = Operation.Method;

                Complete(response);

                return false;
            }

            request = new DeletionRequest(BulkRequest.Request, resourceIdentifier.Identifier,
                BulkRequest.CorrelationIdentifier, BulkRequest.Extensions);

            return true;
        }
    }
}
