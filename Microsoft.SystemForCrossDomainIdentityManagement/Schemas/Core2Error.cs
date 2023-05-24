//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Core2Error : ErrorBase
    {
        // https://datatracker.ietf.org/doc/html/rfc7644#section-3.12
        public Core2Error( string detail, int status, string scimType = null)
        {
            AddSchema(ProtocolSchemaIdentifiers.VERSION_2_ERROR);

            Detail = detail;
            Status = status;
            ScimType = scimType;
        }
    }
}
