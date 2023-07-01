//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Net.Http;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Service.Contracts;

namespace Microsoft.SCIM.Service
{
    internal abstract class BulkOperationContextBase<TPayload> : IBulkOperationContext<TPayload> where TPayload : class
    {
        protected BulkOperationContextBase()
        {
        }

        public IRequest<BulkRequest2> BulkRequest => State.BulkRequest;

        public bool Completed
        {
            get
            {
                if (State == ProcessedState)
                {
                    return true;
                }

                if (State == FaultedState)
                {
                    return true;
                }

                return false;
            }
        }

        public bool Faulted
        {
            get { return State == FaultedState; }
        }

        public IBulkOperationState<TPayload> FaultedState { get; private set; }

        public HttpMethod Method => State.Operation.Method;

        public BulkRequestOperation Operation => State.Operation;

        public IBulkOperationState<TPayload> PreparedState { get; private set; }

        public IBulkOperationState<TPayload> ProcessedState { get; private set; }

        public IBulkOperationState<TPayload> ReceivedState { get; set; }

        public IRequest<TPayload> Request => State.Request;

        public BulkResponseOperation Response => State.Response;

        public IBulkOperationState<TPayload> State { get; set; }

        public void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            State.Complete(response);
        }

        public void Initialize(IBulkOperationState<TPayload> receivedState)
        {
            if (receivedState == null)
            {
                throw new ArgumentNullException(nameof(receivedState));
            }

            if (receivedState.Operation == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidState);
            }

            if (receivedState.BulkRequest == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidState);
            }

            State = ReceivedState = receivedState;
            PreparedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
            FaultedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
            ProcessedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
        }

        public void Prepare(IRequest<TPayload> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            State.Prepare(request);
        }

        public bool TryPrepare() => State.TryPrepare();
    }
}
