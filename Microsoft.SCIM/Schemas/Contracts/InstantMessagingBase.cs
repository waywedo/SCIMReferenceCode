//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas.Contracts
{
    [DataContract]
    public abstract class InstantMessagingBase : TypedValue
    {
        public const string AIM = "aim";
        public const string GTALK = "gtalk";
        public const string ICQ = "icq";
        public const string MSN = "msn";
        public const string QQ = "qq";
        public const string SKYPE = "skype";
        public const string XMPP = "xmpp";
        public const string YAHOO = "yahoo";

        internal InstantMessagingBase()
        {
        }
    }
}