//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Schemas
{
    using System;

    public static class SpecificationVersion
    {
        private static readonly Lazy<Version> _versionOneOhValue = new(() => new Version(1, 0));
        private static readonly Lazy<Version> _versionOneOneValue = new(() => new Version(1, 1));
        private static readonly Lazy<Version> _versionTwoOhValue = new(() => new Version(2, 0));
        // NOTE: This version is to be used for DCaaS only.
        private static readonly Lazy<Version> _versionTwoOhOneValue = new(() => new Version(2, 0, 1));

        public static Version VersionOneOh { get { return _versionOneOhValue.Value; } }

        public static Version VersionOneOne { get { return _versionOneOneValue.Value; } }

        public static Version VersionTwoOhOne { get { return _versionTwoOhOneValue.Value; } }

        public static Version VersionTwoOh { get { return _versionTwoOhValue.Value; } }
    }
}