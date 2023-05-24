﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.SCIM
{
    public abstract class SingularEventToken : EventTokenDecorator
    {
        protected SingularEventToken(IEventToken innerToken) : base(innerToken)
        {
            if (InnerToken.Events.Count != 1)
            {
                throw new ArgumentException(SchemasResources.ExceptionSingleEventExpected);
            }

            var singleEvent = InnerToken.Events.Single();
            SchemaIdentifier = singleEvent.Key;
            Event = new ReadOnlyDictionary<string, object>((IDictionary<string, object>)singleEvent.Value);
        }

        protected SingularEventToken(string serialized) : this(new EventToken(serialized))
        {
        }

        public IReadOnlyDictionary<string, object> Event { get;  }

        public string SchemaIdentifier { get;  }
    }
}