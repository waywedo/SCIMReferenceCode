//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas;

namespace Microsoft.SCIM.Protocol
{
    [DataContract]
    public abstract class QueryResponseBase : Schematized
    {
        [DataMember(Name = ProtocolAttributeNames.RESOURCES, Order = 3)]
        private Resource[] _resources = null;

        protected QueryResponseBase()
        {
            AddSchema(ProtocolSchemaIdentifiers.VERSION_2_LIST_RESPONSE);
        }

        protected QueryResponseBase(IReadOnlyCollection<Resource> resources) : this()
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            _resources = resources.ToArray();
        }

        protected QueryResponseBase(IList<Resource> resources) : this()
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            _resources = resources.ToArray();
        }

        [DataMember(Name = ProtocolAttributeNames.ITEMS_PER_PAGE, Order = 1)]
        public int ItemsPerPage { get; set; }

        public IEnumerable<Resource> Resources
        {
            get { return _resources; }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException(ProtocolResources.ExceptionInvalidValue);
                }

                _resources = value.ToArray();
            }
        }

        [DataMember(Name = ProtocolAttributeNames.START_INDEX, Order = 2)]
        public int? StartIndex { get; set; }

        [DataMember(Name = ProtocolAttributeNames.TOTAL_RESULTS, Order = 0)]
        public int TotalResults { get; set; }
    }

    [DataContract]
    public abstract class QueryResponseBase<TResource> : Schematized where TResource : Resource
    {
        [DataMember(Name = ProtocolAttributeNames.RESOURCES, Order = 3)]
        private TResource[] resources;

        protected QueryResponseBase(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            AddSchema(schemaIdentifier);
            OnInitialization();
        }

        protected QueryResponseBase(string schemaIdentifier, IReadOnlyCollection<TResource> resources)
            : this(schemaIdentifier)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        protected QueryResponseBase(string schemaIdentifier, IList<TResource> resources)
            : this(schemaIdentifier)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        [DataMember(Name = ProtocolAttributeNames.ITEMS_PER_PAGE, Order = 1)]
        public int ItemsPerPage { get; set; }

        public IEnumerable<TResource> Resources
        {
            get { return resources; }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException(
                        ProtocolResources.ExceptionInvalidValue
                    );
                }
                resources = value.ToArray();
            }
        }

        [DataMember(Name = ProtocolAttributeNames.START_INDEX, Order = 2)]
        public int? StartIndex { get; set; }

        [DataMember(Name = ProtocolAttributeNames.TOTAL_RESULTS, Order = 0)]
        public int TotalResults { get; set; }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            OnInitialization();
        }

        private void OnInitialization()
        {
            resources = Array.Empty<TResource>();
        }
    }
}