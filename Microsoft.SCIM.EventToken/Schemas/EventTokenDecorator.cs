//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.SCIM.EventToken.Schemas
{
    public abstract class EventTokenDecorator : IEventToken
    {
        protected EventTokenDecorator(IEventToken innerToken)
        {
            InnerToken = innerToken ?? throw new ArgumentNullException(nameof(innerToken));
        }

        protected EventTokenDecorator(string serialized) : this(new EventToken(serialized))
        {
        }

        public IReadOnlyCollection<string> Audience
        {
            get { return InnerToken.Audience; }
            set { InnerToken.Audience = value; }
        }

        public IDictionary<string, object> Events
        {
            get { return InnerToken.Events; }
        }

        public DateTime? Expiration
        {
            get { return InnerToken.Expiration; }
            set { InnerToken.Expiration = value; }
        }

        public JwtHeader Header
        {
            get { return InnerToken.Header; }
        }

        public string Identifier
        {
            get { return InnerToken.Identifier; }
        }

        public IEventToken InnerToken { get; }

        public DateTime IssuedAt
        {
            get { return InnerToken.IssuedAt; }
        }

        public string Issuer
        {
            get { return InnerToken.Issuer; }
        }

        public DateTime? NotBefore
        {
            get { return InnerToken.NotBefore; }
        }

        public string Subject
        {
            get { return InnerToken.Subject; }
            set { throw new NotImplementedException(); }
        }

        public string Transaction
        {
            get { return InnerToken.Transaction; }
            set { InnerToken.Transaction = value; }
        }
    }
}