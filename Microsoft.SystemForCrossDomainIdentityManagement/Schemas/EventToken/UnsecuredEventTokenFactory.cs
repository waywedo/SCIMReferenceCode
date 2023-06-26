//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.SCIM.Schemas.EventToken;

namespace Microsoft.SCIM
{
    public abstract class UnsecuredEventTokenFactory : EventTokenFactory
    {
        private static readonly Lazy<JwtHeader> _unsecuredTokenHeader = new(() => ComposeHeader());

        protected UnsecuredEventTokenFactory(string issuer) : base(issuer, _unsecuredTokenHeader.Value)
        {
        }

        private static JwtHeader ComposeHeader()
        {
            return new JwtHeader
            {
                { EventToken.HEADER_KEY_ALGORITHM, EventToken.JWT_ALGORITHM_NONE }
            };
        }
    }
}