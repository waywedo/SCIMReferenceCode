//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class PhoneNumberBase : TypedValue
    {
        public const string FAX = "fax";
        public const string HOME = "home";
        public const string MOBILE = "mobile";
        public const string OTHER = "other";
        public const string PAGER = "pager";
        public const string WORK = "work";

        internal PhoneNumberBase()
        {
        }
    }
}
