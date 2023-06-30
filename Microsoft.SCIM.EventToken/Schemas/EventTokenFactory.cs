//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.SCIM.EventToken.Schemas
{
    public abstract class EventTokenFactory
    {
        protected EventTokenFactory(string issuer, JwtHeader header)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            Issuer = issuer;
            Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        public JwtHeader Header { get; }

        public string Issuer { get; }

        public abstract IEventToken Create(IDictionary<string, object> events);
    }
}