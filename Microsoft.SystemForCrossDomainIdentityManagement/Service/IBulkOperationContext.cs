//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Net.Http;

namespace Microsoft.SCIM
{
    public interface IBulkOperationContext : IBulkOperationState
    {
        bool Completed { get; }
        bool Faulted { get; }
        HttpMethod Method { get; }
    }

    public interface IBulkOperationContext<TPayload> : IBulkOperationContext,
        IBulkOperationState<TPayload> where TPayload : class
    {
        IBulkOperationState<TPayload> FaultedState { get; }
        IBulkOperationState<TPayload> PreparedState { get; }
        IBulkOperationState<TPayload> ProcessedState { get; }
        IBulkOperationState<TPayload> ReceivedState { get; }
        IBulkOperationState<TPayload> State { get; set; }
    }
}
