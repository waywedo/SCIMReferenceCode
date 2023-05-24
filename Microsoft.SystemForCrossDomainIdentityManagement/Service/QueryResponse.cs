// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
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
