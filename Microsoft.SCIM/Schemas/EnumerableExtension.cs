//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SCIM
{
    internal static class EnumerableExtension
    {
        public static int CheckedCount<T>(this IEnumerable<T> enumeration)
        {
            var longCount = enumeration.LongCount();
            return Convert.ToInt32(longCount);
        }
    }
}
