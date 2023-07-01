//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public sealed class AuthenticationScheme
    {
        private const string TOKEN_TYPE = "oauthbearertoken";
        private const string TOKEN_NAME = "OAuth Bearer Token";
        private const string TOKEN_DESCRIPTION =
            "Authentication Scheme using the OAuth Bearer Token Standard";
        private const string DOCUMENTATION_RESOURCE = "http://example.com/help/oauth.html";
        private const string SPECIFICATION_RESOURCE = "http://tools.ietf.org/html/draft-ietf-oauth-v2-bearer-01";

        private static readonly Lazy<Uri> DocumentationUri = new(() => new Uri(DOCUMENTATION_RESOURCE));
        private static readonly Lazy<Uri> SpecificationUri = new(() => new Uri(SPECIFICATION_RESOURCE));

        [DataMember(Name = AttributeNames.TYPE)]
        public string AuthenticationType { get; set; }

        [DataMember(Name = AttributeNames.DESCRIPTION)]
        public string Description { get; set; }

        [DataMember(Name = AttributeNames.DOCUMENTATION)]
        public Uri DocumentationResource { get; set; }

        [DataMember(Name = AttributeNames.NAME)]
        public string Name { get; set; }

        [DataMember(Name = AttributeNames.PRIMARY)]
        public bool Primary { get; set; }

        [DataMember(Name = AttributeNames.SPECIFICATION)]
        public Uri SpecificationResource { get; set; }

        public static AuthenticationScheme CreateOpenStandardForAuthorizationBearerTokenScheme()
        {
            return new AuthenticationScheme()
            {
                AuthenticationType = TOKEN_TYPE,
                Name = TOKEN_NAME,
                Description = TOKEN_DESCRIPTION,
                DocumentationResource = DocumentationUri.Value,
                SpecificationResource = SpecificationUri.Value
            };
        }
    }
}