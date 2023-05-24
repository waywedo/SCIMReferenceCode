using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.SCIM
{
    internal class BulkCreationOperationState : BulkOperationStateBase<Resource>, IBulkCreationOperationState
    {
        private const string RELATIVE_RESOURCE_IDENTIFIER_TEMPLATE = "/{0}/{1}";

        private readonly List<IBulkUpdateOperationContext> _dependents;
        private readonly IRequest<Resource> _creationRequest;
        private readonly List<IBulkUpdateOperationContext> _subordinates;
        private readonly IBulkCreationOperationContext _typedContext;

        public BulkCreationOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation,
            IBulkCreationOperationContext context) : base(request, operation, context)
        {
            _typedContext = context;

            _dependents = new List<IBulkUpdateOperationContext>();
            Dependents = _dependents.AsReadOnly();

            _subordinates = new List<IBulkUpdateOperationContext>();
            Subordinates = _subordinates.AsReadOnly();

            if (BulkRequest.BaseResourceIdentifier == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            if (Operation.Data == null)
            {
                var invalidOperationExceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionInvalidOperationTemplate,
                    operation.Identifier);

                throw new ArgumentException(invalidOperationExceptionMessage);
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(operation.Data.ToString());

            if (operationDataJson.schemas != null)
            {
                var operationData = operation.Data;
                Resource resource = null;

                if (operationData.IsResourceType(SchemaIdentifiers.CORE_2_USER))
                {
                    var user = operationDataJson.ToObject<Core2EnterpriseUser>();
                    resource = user;

                    if (user.EnterpriseExtension.Manager != null)
                    {
                        var resourceIdentifier = string.Format(CultureInfo.InvariantCulture,
                            RELATIVE_RESOURCE_IDENTIFIER_TEMPLATE,
                            ProtocolConstants.PATH_USERS,
                            Operation.Identifier
                        );
                        var patchResourceIdentifier = new Uri(resourceIdentifier, UriKind.Relative);

                        var patchOperation = PatchOperation2Combined.Create(OperationName.Add,
                            AttributeNames.MANAGER, user.EnterpriseExtension.Manager.Value);
                        var patchRequest = new PatchRequest2();

                        patchRequest.AddOperation(patchOperation);

                        AddSubordinate(patchResourceIdentifier, patchRequest, context);

                        user.EnterpriseExtension.Manager = null;
                    }
                }

                if (operationData.IsResourceType(SchemaIdentifiers.CORE_2_GROUP))
                {
                    var group = operationDataJson.ToObject<Core2Group>();
                    resource = group;

                    if (group.Members?.Any() == true)
                    {
                        var resourceIdentifier = string.Format(CultureInfo.InvariantCulture,
                            RELATIVE_RESOURCE_IDENTIFIER_TEMPLATE,
                            ProtocolConstants.PATH_GROUPS,
                            Operation.Identifier
                        );
                        var patchResourceIdentifier = new Uri(resourceIdentifier, UriKind.Relative);
                        var patchRequest = new PatchRequest2();

                        foreach (Member member in group.Members)
                        {
                            if (member == null || string.IsNullOrWhiteSpace(member.Value))
                            {
                                continue;
                            }

                            var memberValue = System.Text.Json.JsonSerializer.Serialize(member);
                            if (!string.IsNullOrWhiteSpace(memberValue))
                            {
                                var patchOperation = PatchOperation2Combined.Create(OperationName.Add,
                                    AttributeNames.MEMBERS, memberValue);

                                patchRequest.AddOperation(patchOperation);
                            }
                        }

                        AddSubordinate(patchResourceIdentifier, patchRequest, context);

                        group.Members = null;
                    }
                }

                if (resource == null)
                {
                    var invalidOperationExceptionMessage = string.Format(CultureInfo.InvariantCulture,
                        ServiceResources.ExceptionInvalidOperationTemplate,
                        operation.Identifier);

                    throw new ArgumentException(invalidOperationExceptionMessage);
                }

                _creationRequest = new CreationRequest(request.Request, resource, request.CorrelationIdentifier,
                    request.Extensions);
            }
            else
            {
                var invalidOperationExceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    ServiceResources.ExceptionInvalidOperationTemplate,
                    operation.Identifier);

                throw new ArgumentException(invalidOperationExceptionMessage);
            }
        }

        public IReadOnlyCollection<IBulkUpdateOperationContext> Dependents { get; }

        public IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates { get; }

        public void AddDependent(IBulkUpdateOperationContext dependent)
        {
            if (dependent == null)
            {
                throw new ArgumentNullException(nameof(dependent));
            }

            if (Context.State != Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidState);
            }

            _dependents.Add(dependent);
        }

        public void AddSubordinate(IBulkUpdateOperationContext subordinate)
        {
            if (subordinate == null)
            {
                throw new ArgumentNullException(nameof(subordinate));
            }

            if (Context.State != Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidState);
            }

            _subordinates.Add(subordinate);
        }

        private void AddSubordinate(Uri resourceIdentifier, PatchRequest2 patchRequest, IBulkCreationOperationContext context)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            if (patchRequest == null)
            {
                throw new ArgumentNullException(nameof(patchRequest));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var bulkPatchOperation = BulkRequestOperation.CreatePatchOperation(resourceIdentifier, patchRequest);
            var patchOperationContext = new BulkUpdateOperationContext(BulkRequest, bulkPatchOperation, context);

            AddSubordinate(patchOperationContext);
        }

        public override void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!_typedContext.Subordinates.Any())
            {
                base.Complete(response);
                return;
            }

            if (Context.State != Context.PreparedState && _typedContext.State != _typedContext.PendingState)
            {
                throw new InvalidOperationException(
                    ServiceResources.ExceptionInvalidStateTransition);
            }

            IBulkOperationState<Resource> nextState;

            if (response.Response is ErrorResponse)
            {
                nextState = Context.FaultedState;
            }
            else if (_typedContext.Subordinates.Any((IBulkUpdateOperationContext item) => !item.Completed))
            {
                nextState = _typedContext.PendingState;
            }
            else
            {
                nextState = Context.ProcessedState;
            }

            if (this == nextState)
            {
                if (this != _typedContext.PendingState || Response == null)
                {
                    Response = response;
                }
                Context.State = this;
            }
            else
            {
                nextState.Complete(response);
            }
        }

        public override bool TryPrepareRequest(out IRequest<Resource> request)
        {
            request = _creationRequest;
            return true;
        }
    }
}
