//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public class PluralUnsecuredEventTokenFactory : UnsecuredEventTokenFactory
    {
        public PluralUnsecuredEventTokenFactory(string issuer) : base(issuer)
        {
        }

        public override IEventToken Create(IDictionary<string, object> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            return new EventToken(Issuer, Header, events);
        }
    }
}