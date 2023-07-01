//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public abstract class PatchRequest2DeserializingFactory<TPatchRequest, TOperation> :
        ProtocolJsonDeserializingFactory<TPatchRequest>,
        ISchematizedJsonDeserializingFactory<TPatchRequest>
        where TOperation : PatchOperation2Base
        where TPatchRequest : PatchRequest2Base<TOperation>
    {
        public override TPatchRequest Create(IReadOnlyDictionary<string, object> json)
        {
            var normalized = Normalize(json).ToDictionary(
                (item) => item.Key,
                (item) => item.Value
            );

            if (normalized.TryGetValue(ProtocolAttributeNames.OPERATIONS, out object operations))
            {
                normalized.Remove(ProtocolAttributeNames.OPERATIONS);
            }

            var result = base.Create(normalized);

            if (operations != null)
            {
                foreach (var patchOperation in Deserialize(operations))
                {
                    result.AddOperation(patchOperation as TOperation);
                }
            }

            return result;
        }

        private static bool TryDeserialize(Dictionary<string, object> json, out PatchOperation2Base operation)
        {
            operation = null;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (!json.TryGetValue(AttributeNames.VALUE, out object value))
            {
                return false;
            }

            switch (value)
            {
                case string:
                    operation = new PatchOperation2SingleValuedJsonDeserializingFactory().Create(json);
                    return true;
                case ArrayList:
                case object:
                    operation = new PatchOperation2JsonDeserializingFactory().Create(json);
                    return true;
                default:
                    var unsupported = value.GetType().FullName;
                    throw new NotSupportedException(unsupported);
            }
        }

        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(ArrayList operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            var result = new List<PatchOperation2Base>(operations.Count);

            foreach (Dictionary<string, object> json in operations)
            {
                if (TryDeserialize(json, out PatchOperation2Base patchOperation))
                {
                    result.Add(patchOperation);
                }
            }

            return result;
        }

        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(object[] operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            var result = new List<PatchOperation2Base>(operations.Length);

            foreach (var json in operations.Cast<Dictionary<string, object>>())
            {
                if (TryDeserialize(json, out PatchOperation2Base patchOperation))
                {
                    result.Add(patchOperation);
                }
            }

            return result;
        }

        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(object operations)
        {
            switch (operations)
            {
                case ArrayList list:
                    return Deserialize(list);
                case object[] array:
                    return Deserialize(array);
                default:
                    var unsupported = operations.GetType().FullName;
                    throw new NotSupportedException(unsupported);
            }
        }
    }
}