//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.SCIM.Protocol.Contracts;

namespace Microsoft.SCIM.Protocol
{
    public class PaginationParameters : IPaginationParameters
    {
        private int? _count;
        private int? _startIndex;

        public int? Count
        {
            get { return _count; }

            set
            {
                if (value < 0)
                {
                    _count = 0;
                    return;
                }
                _count = value;
            }
        }

        public int? StartIndex
        {
            get { return _startIndex; }

            set
            {
                if (value < 1)
                {
                    _startIndex = 1;
                    return;
                }
                _startIndex = value;
            }
        }
    }
}