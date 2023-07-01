//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ElectronicMailAddressBase : TypedValue
    {
        internal ElectronicMailAddressBase()
        {
        }

        public const string HOME = "home";
        public const string OTHER = "other";
        public const string WORK = "work";
    }
}