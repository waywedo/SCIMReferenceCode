//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;

namespace Microsoft.SCIM.Schemas
{
    internal static class DateTimeExtension
    {
        private const string FORMAT_STRING_ROUNDTRIP = "O";

        public static string ToRoundtripString(this DateTime dateTime)
        {
            return dateTime.ToString(FORMAT_STRING_ROUNDTRIP, CultureInfo.InvariantCulture);
        }
    }
}
