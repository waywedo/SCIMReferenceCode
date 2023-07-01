//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Protocol
{
    [DataContract]
    public abstract class PatchRequest2Base<TOperation> : PatchRequestBase where TOperation : PatchOperation2Base
    {
        [DataMember(Name = ProtocolAttributeNames.OPERATIONS, Order = 2)]
        private List<TOperation> _operationsValue;
        private IReadOnlyCollection<TOperation> _operationsWrapper;

        protected PatchRequest2Base()
        {
            OnInitialization();
            OnInitialized();
            AddSchema(ProtocolSchemaIdentifiers.VERSION_2_PATCH_OPERATION);
        }

        protected PatchRequest2Base(IReadOnlyCollection<TOperation> operations) : this()
        {
            _operationsValue.AddRange(operations);
        }

        public IReadOnlyCollection<TOperation> Operations
        {
            get { return _operationsWrapper; }
        }

        public void AddOperation(TOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _operationsValue.Add(operation);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            OnInitialized();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _operationsValue = new List<TOperation>();
        }

        private void OnInitialized()
        {
            _operationsWrapper = _operationsValue.AsReadOnly();
        }
    }
}
