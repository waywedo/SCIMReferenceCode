//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class Resource : Schematized
    {
        [DataMember(Name = AttributeNames.EXTERNAL_IDENTIFIER, IsRequired = false, EmitDefaultValue = false)]
        public string ExternalIdentifier { get; set; }

        [DataMember(Name = AttributeNames.IDENTIFIER)]
        public string Identifier { get; set; }

        public virtual bool TryGetIdentifier(Uri baseIdentifier, out Uri identifier)
        {
            identifier = null;
            return false;
        }

        public virtual bool TryGetPathIdentifier(out Uri pathIdentifier)
        {
            pathIdentifier = null;
            return false;
        }
    }
}