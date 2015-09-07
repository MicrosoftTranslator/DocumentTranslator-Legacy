// -
// <copyright file="TmxTagType.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Enumerator defining the types of tags in TMX file.
    /// </summary>
    public enum TmxTagType
    {
        XML,
        DOCTYPE,
        TMX_OPEN,
        TMX_CLOSE,
        BODY_OPEN,
        BODY_CLOSE,
        HEADER,
        TU,
        NONE
    }
}
