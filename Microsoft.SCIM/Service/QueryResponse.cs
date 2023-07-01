// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.SCIM.Protocol;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Service
{
    [DataContract]
    public sealed class QueryResponse : QueryResponseBase
    {
        public QueryResponse()
        {
        }

        public QueryResponse(IReadOnlyCollection<Resource> resources)
            : base(resources)
        {
        }

        public QueryResponse(IList<Resource> resources)
            : base(resources)
        {
        }
    }
}
