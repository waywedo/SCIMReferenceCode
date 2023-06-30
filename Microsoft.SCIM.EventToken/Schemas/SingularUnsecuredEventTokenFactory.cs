//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM.EventToken.Schemas
{
    public class SingularUnsecuredEventTokenFactory : UnsecuredEventTokenFactory
    {
        public SingularUnsecuredEventTokenFactory(string issuer, string eventSchemaIdentifier) : base(issuer)
        {
            if (string.IsNullOrWhiteSpace(eventSchemaIdentifier))
            {
                throw new ArgumentNullException(nameof(eventSchemaIdentifier));
            }

            EventSchemaIdentifier = eventSchemaIdentifier;
        }

        private string EventSchemaIdentifier { get; }

        public override IEventToken Create(IDictionary<string, object> events)
        {
            var tokenEvents = new Dictionary<string, object>(1)
            {
                { EventSchemaIdentifier, events }
            };

            return new EventToken(Issuer, Header, tokenEvents);
        }
    }
}