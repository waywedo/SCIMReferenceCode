//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Globalization;
using Microsoft.SCIM.Schemas.Contracts;

namespace Microsoft.SCIM.Schemas
{
    // Refer to https://en.wikipedia.org/wiki/Unix_time
    public class UnixTime : IUnixTime
    {
        public static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public UnixTime(long epochTimestamp)
        {
            EpochTimestamp = epochTimestamp;
        }

        public UnixTime(int epochTimestamp) : this(Convert.ToInt64(epochTimestamp))
        {
        }

        public UnixTime(double epochTimestamp) : this(Convert.ToInt64(epochTimestamp))
        {
        }

        public UnixTime(TimeSpan sinceEpoch) : this(sinceEpoch.TotalSeconds)
        {
        }

        public UnixTime(DateTime dateTime) : this(dateTime.ToUniversalTime().Subtract(Epoch))
        {
        }

        public DateTime ToUniversalTime()
        {
            return Epoch.AddSeconds(EpochTimestamp);
        }

        public long EpochTimestamp { get; }

        public override string ToString()
        {
            return EpochTimestamp.ToString(CultureInfo.InvariantCulture);
        }
    }
}
