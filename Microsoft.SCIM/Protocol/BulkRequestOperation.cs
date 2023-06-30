//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public sealed class BulkRequestOperation : BulkOperation
    {
        private Uri _path;
        [DataMember(Name = ProtocolAttributeNames.PATH, Order = 0)]
        private string _pathValue;

        private BulkRequestOperation()
        {
        }

        [DataMember(Name = ProtocolAttributeNames.DATA, Order = 4)]
        public object Data { get; set; }

        public Uri Path
        {
            get { return _path; }
            set
            {
                _path = value;
                _pathValue = new SCIMResourceIdentifier(value).RelativePath;
            }
        }

        public static BulkRequestOperation CreateDeleteOperation(Uri resource)
        {
            return new BulkRequestOperation
            {
                Method = HttpMethod.Delete,
                Path = resource ?? throw new ArgumentNullException(nameof(resource))
            };
        }

        public static BulkRequestOperation CreatePatchOperation(Uri resource, PatchRequest2 data)
        {
            ArgumentNullException.ThrowIfNull(resource, nameof(resource));
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            var patchRequest = new PatchRequest2(data.Operations);

            return new BulkRequestOperation
            {
                Method = ProtocolExtensions.PatchMethod,
                Path = resource,
                Data = patchRequest
            };
        }

        public static BulkRequestOperation CreatePostOperation(Resource data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            if (data.Schemas == null)
            {
                throw new ArgumentException(ProtocolResources.ExceptionUnidentifiableSchema);
            }

            if (!data.Schemas.Any())
            {
                throw new ArgumentException(ProtocolResources.ExceptionUnidentifiableSchema);
            }

            var paths = new List<Uri>(1);

            foreach (var schemaIdentifier in data.Schemas.Select((string item) => new SchemaIdentifier(item)))
            {
                Uri schemaIdentifierPath = null;

                if (schemaIdentifier.TryFindPath(out string pathValue))
                {
                    schemaIdentifierPath = new Uri(pathValue, UriKind.Relative);

                    if (!paths.Any((Uri item) => Uri.Compare(item, schemaIdentifierPath, UriComponents.AbsoluteUri,
                        UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        paths.Add(schemaIdentifierPath);
                    }
                }

                if (data.TryGetPathIdentifier(out Uri resourcePath))
                {
                    if(!paths.Any((Uri item) => Uri.Compare(item, resourcePath, UriComponents.AbsoluteUri, UriFormat.UriEscaped,
                        StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        paths.Add(resourcePath);
                    }
                }
            }

            if (paths.Count != 1)
            {
                throw new NotSupportedException(string.Join(Environment.NewLine, data.Schemas));
            }

            return new BulkRequestOperation
            {
                _path = paths.Single(),
                Method = HttpMethod.Post,
                Data = data
            };
        }

        private void InitializePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _path = null;
                return;
            }

            _path = new Uri(value, UriKind.Relative);
        }

        private void InitializePath()
        {
            InitializePath(_pathValue);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            InitializePath();
        }
    }
}
