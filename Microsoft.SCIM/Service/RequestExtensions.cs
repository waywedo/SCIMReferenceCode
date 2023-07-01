// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;
using Microsoft.SCIM.Service.Contracts;
using Newtonsoft.Json;

namespace Microsoft.SCIM.Service
{
    public static class RequestExtensions
    {
        private const string SEGMENT_INTERFACE = SEGMENT_SEPARATOR + SchemaConstants.PATH_INTERFACE + SEGMENT_SEPARATOR;
        private const string SEGMENT_SEPARATOR = "/";
        private static readonly Lazy<char[]> SegmentSeparators = new(() => SEGMENT_SEPARATOR.ToArray());

        public static Uri GetBaseResourceIdentifier(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            var lastSegment = request.Path.Value.Split(SegmentSeparators.Value, StringSplitOptions.RemoveEmptyEntries).Last();

            if (string.Equals(lastSegment, SchemaConstants.PATH_INTERFACE, StringComparison.OrdinalIgnoreCase))
            {
                return new Uri(request.GetDisplayUrl());
            }

            var resourceIdentifier = request.GetDisplayUrl();
            var indexInterface = resourceIdentifier.LastIndexOf(SEGMENT_INTERFACE, StringComparison.OrdinalIgnoreCase);

            if (indexInterface < 0)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            var baseResource = resourceIdentifier[..indexInterface];

            return new Uri(baseResource, UriKind.Absolute);
        }

        public static bool TryGetRequestIdentifier(this HttpRequest request, out string requestIdentifier)
        {
            request?.Headers.TryGetValue("client-id", out _);
            requestIdentifier = Guid.NewGuid().ToString();
            return true;
        }

        public static Queue<IBulkOperationContext> EnqueueOperations(this IRequest<BulkRequest2> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidRequest);
            }

            var creations = new List<IBulkCreationOperationContext>();
            var updates = new List<IBulkUpdateOperationContext>();
            var operations = new List<IBulkOperationContext>();

            foreach (BulkRequestOperation operation in request.Payload.Operations)
            {
                request.Enlist(operation, operations, creations, updates);
            }

            var result = new Queue<IBulkOperationContext>(operations.Count);

            foreach (IBulkOperationContext operation in operations)
            {
                result.Enqueue(operation);
            }

            return result;
        }

        private static void Relate(this IBulkUpdateOperationContext context, IEnumerable<IBulkCreationOperationContext> creations)
        {
            if (creations == null)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (context.Method == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidContext);
            }

            if (context.Operation == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidContext);
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(context.Operation.Data.ToString());
            var patchOperations = operationDataJson.Operations.ToObject<List<PatchOperation2Combined>>();
            var patchRequest = new PatchRequest2(patchOperations);

            foreach (IBulkCreationOperationContext creation in creations)
            {
                if (creation.Operation == null)
                {
                    throw new InvalidOperationException(
                        ServiceResources.ExceptionInvalidOperation);
                }

                if (string.IsNullOrWhiteSpace(creation.Operation.Identifier))
                {
                    throw new InvalidOperationException(
                        ServiceResources.ExceptionInvalidOperation);
                }

                if (patchRequest.References(creation.Operation.Identifier))
                {
                    creation.AddDependent(context);
                    context.AddDependency(creation);
                }
            }
        }

        private static void Enlist(this IRequest<BulkRequest2> request, BulkRequestOperation operation,
            List<IBulkOperationContext> operations, List<IBulkCreationOperationContext> creations,
            List<IBulkUpdateOperationContext> updates)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            if (creations == null)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (updates == null)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (operation.Method == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            if (HttpMethod.Post == operation.Method)
            {
                var context = new BulkCreationOperationContext(request, operation);
                context.Relate(updates);

                (IBulkOperationContext item, int index) firstDependent =
                    operations.Select((item, index) => (item, index))
                        .Where(candidateItem => context.Dependents.Any(dependentItem => dependentItem == candidateItem.item))
                        .OrderBy((item) => item.index)
                        .FirstOrDefault();

                if (firstDependent != default)
                {
                    operations.Insert(firstDependent.index, context);
                }
                else
                {
                    operations.Add(context);
                }

                creations.Add(context);
                operations.AddRange(context.Subordinates);
                updates.AddRange(context.Subordinates);

                return;
            }

            if (HttpMethod.Delete == operation.Method)
            {
                var context = new BulkDeletionOperationContext(request, operation);
                operations.Add(context);

                return;
            }

            if (ProtocolExtensions.PatchMethod == operation.Method)
            {
                var context = new BulkUpdateOperationContext(request, operation);
                context.Relate(creations);
                operations.Add(context);
                updates.Add(context);

                return;
            }

            throw new InvalidOperationException();
        }

        private static void Relate(this IBulkCreationOperationContext context, IEnumerable<IBulkUpdateOperationContext> updates)
        {
            if (updates == null)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (context.Method == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidContext);
            }

            if (context.Operation == null)
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidContext);
            }

            if (string.IsNullOrWhiteSpace(context.Operation.Identifier))
            {
                throw new ArgumentException(ServiceResources.ExceptionInvalidOperation);
            }

            foreach (IBulkUpdateOperationContext update in updates)
            {
                switch (update.Operation.Data)
                {
                    case PatchRequest2 patchRequest:
                        if (patchRequest.References(context.Operation.Identifier))
                        {
                            context.AddDependent(update);
                            update.AddDependency(context);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
