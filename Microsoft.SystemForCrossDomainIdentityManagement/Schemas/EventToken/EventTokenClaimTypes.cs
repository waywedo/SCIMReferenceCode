//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas.EventToken
{
    public static class EventTokenClaimTypes
    {
        public const string AUDIENCE = "aud";
        public const string EXPIRATION = "exp";
        public const string EVENTS = "events";
        public const string IDENTIFIER = "jti";
        public const string ISSUED_AT = "iat";
        public const string ISSUER = "iss";
        public const string NOT_BEFORE = "nbf";
        public const string SUBJECT = "sub";
        public const string TRANSACTION = "txn";
    }
}