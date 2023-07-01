//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;
using System.Linq;
using Microsoft.SCIM.Resources;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Schemas
{
    public sealed class SCIMResourceIdentifier : ISCIMResourceIdentifier
    {
        private const string SEPARATOR_SEGMENTS = "/";

        private static readonly Lazy<string[]> SeperatorsSegments = new(() => new string[] { SEPARATOR_SEGMENTS });

        public SCIMResourceIdentifier(Uri identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var path = identifier.OriginalString;

            // System.Uri.Segments is not supported for relative identifiers.
            var segmentsIndexed = path.Split(SeperatorsSegments.Value, StringSplitOptions.None)
                .Select((item, index) => new { Segment = item, Index = index })
                .ToArray();

            var segmentSystemForCrossDomainIdentityManagement = segmentsIndexed
                .LastOrDefault((item) => item.Segment.Equals(SchemaConstants.PATH_INTERFACE, StringComparison.OrdinalIgnoreCase));

            if (segmentSystemForCrossDomainIdentityManagement == null)
            {
                if (identifier.IsAbsoluteUri)
                {
                    var exceptionMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        SchemasResources.ExceptionInvalidIdentifierTemplate,
                        path
                    );
                    throw new ArgumentException(exceptionMessage);
                }
            }
            else
            {
                segmentsIndexed = segmentsIndexed.Where(
                    item => item.Index > segmentSystemForCrossDomainIdentityManagement.Index
                ).ToArray();
            }

            var segmentsRelative = segmentsIndexed.Select((item) => item.Segment).ToArray();
            var relativePath = string.Join(SEPARATOR_SEGMENTS, segmentsRelative);

            if (!relativePath.StartsWith(SEPARATOR_SEGMENTS, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = string.Concat(SEPARATOR_SEGMENTS, relativePath);
            }

            RelativePath = relativePath;
        }

        public string RelativePath { get; }
    }
}
