//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM.Protocol
{
    public enum ErrorType
    {
        invalidFilter,
        invalidPath,
        invalidSyntax,
        invalidValue,
        invalidVers,
        mutability,
        noTarget,
        sensitive,
        tooMany,
        uniqueness
    }
}