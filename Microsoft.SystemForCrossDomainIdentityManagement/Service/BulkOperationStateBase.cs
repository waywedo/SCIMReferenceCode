//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;

namespace Microsoft.SCIM
{
    internal abstract class BulkOperationStateBase<TPayload> : IBulkOperationState<TPayload> where TPayload : class
    {
        protected BulkOperationStateBase(IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkOperationContext<TPayload> context)
        {
            BulkRequest = request ?? throw new ArgumentNullException(nameof(request));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRequest<BulkRequest2> BulkRequest { get; }

        public IBulkOperationContext<TPayload> Context { get; }

        public BulkRequestOperation Operation { get; set; }

        public IRequest<TPayload> Request { get; private set; }

        public BulkResponseOperation Response { get; set; }

        public virtual void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var errorResponse = response.Response as ErrorResponse;

            if (Context.State != Context.PreparedState && errorResponse == null)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            IBulkOperationState<TPayload> completionState;

            if (errorResponse != null)
            {
                completionState = Context.FaultedState;
            }
            else
            {
                completionState = Context.ProcessedState;
            }

            if (this == completionState)
            {
                Response = response;
                Context.State = this;
            }
            else
            {
                completionState.Complete(response);
            }
        }

        public virtual void Prepare(IRequest<TPayload> request)
        {
            if (Context.State != Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            if (this != Context.PreparedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            Request = request ?? throw new ArgumentNullException(nameof(request));
            Context.State = this;
        }

        public virtual bool TryPrepare()
        {
            if (Context.State != Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            if (!TryPrepareRequest(out IRequest<TPayload> request))
            {
                if (Context.State != Context.FaultedState)
                {
                    Context.State = Context.FaultedState;
                }

                return false;
            }

            Context.PreparedState.Prepare(request);

            return true;
        }

        public abstract bool TryPrepareRequest(out IRequest<TPayload> request);
    }
}
