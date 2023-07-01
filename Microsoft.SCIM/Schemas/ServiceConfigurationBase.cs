//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.SCIM.Resources;

namespace Microsoft.SCIM.Schemas
{
    [DataContract]
    public abstract class ServiceConfigurationBase : Schematized
    {
        [DataMember(Name = AttributeNames.AUTHENTICATION_SCHEMES)]
        private List<AuthenticationScheme> _authenticationSchemes;
        private IReadOnlyCollection<AuthenticationScheme> _authenticationSchemesWrapper;
        private object _thisLock;

        protected ServiceConfigurationBase()
        {
            OnInitialization();
            OnInitialized();
        }

        public IReadOnlyCollection<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _authenticationSchemesWrapper; }
        }

        [DataMember(Name = AttributeNames.BULK)]
        public BulkRequestsFeature BulkRequests { get; set; }

        [DataMember(Name = AttributeNames.DOCUMENTATION)]
        public Uri DocumentationResource { get; set; }

        [DataMember(Name = AttributeNames.ENTITY_TAG)]
        public Feature EntityTags { get; set; }

        [DataMember(Name = AttributeNames.FILTER)]
        public Feature Filtering { get; set; }

        [DataMember(Name = AttributeNames.CHANGE_PASSWORD)]
        public Feature PasswordChange { get; set; }

        [DataMember(Name = AttributeNames.PATCH)]
        public Feature Patching { get; set; }

        [DataMember(Name = AttributeNames.SORT)]
        public Feature Sorting { get; set; }

        public void AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            if (string.IsNullOrWhiteSpace(authenticationScheme.Name))
            {
                throw new ArgumentException(
                    SchemasResources.ExceptionUnnamedAuthenticationScheme);
            }

            var containsFunction = new Func<bool>(() =>
                _authenticationSchemes.Any(item => string.Equals(item.Name, authenticationScheme.Name, StringComparison.OrdinalIgnoreCase))
            );

            if (!containsFunction())
            {
                lock (_thisLock)
                {
                    if (!containsFunction())
                    {
                        _authenticationSchemes.Add(authenticationScheme);
                    }
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            OnInitialized();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            _thisLock = new object();
            _authenticationSchemes = new List<AuthenticationScheme>();
        }

        private void OnInitialized()
        {
            _authenticationSchemesWrapper = _authenticationSchemes.AsReadOnly();
        }
    }
}