//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    [DataContract]
    public abstract class BulkOperations<TOperation> : Schematized where TOperation : BulkOperation
    {
        [DataMember(Name = ProtocolAttributeNames.OPERATIONS, Order = 2)]
        private List<TOperation> _operations;
        private IReadOnlyCollection<TOperation> _operationsWrapper;
        private object _thisLock;

        protected BulkOperations(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            AddSchema(schemaIdentifier);
            OnInitialization();
            OnInitialized();
        }

        public IReadOnlyCollection<TOperation> Operations => _operationsWrapper;

        public void AddOperation(TOperation operation)
        {
            ArgumentNullException.ThrowIfNull(operation, nameof(operation));

            if (string.IsNullOrWhiteSpace(operation.Identifier))
            {
                throw new ArgumentException(
                    ProtocolResources.ExceptionUnidentifiableOperation);
            }

            if (!OperationExists(operation.Identifier))
            {
                lock (_thisLock)
                {
                    if (!OperationExists(operation.Identifier))
                    {
                        _operations.Add(operation);
                    }
                }
            }
        }

        private bool OperationExists(string identifier)
        {
            return _operations.Any((BulkOperation item) => string.Equals(item.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _) => OnInitialized();

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _) => OnInitialization();

        private void OnInitialization()
        {
            _thisLock = new object();
            _operations = new List<TOperation>();
        }

        private void OnInitialized()
        {
            _operationsWrapper = _operations.AsReadOnly();
        }
    }
}
