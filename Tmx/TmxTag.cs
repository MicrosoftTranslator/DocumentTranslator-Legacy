// -
// <copyright file="TmxTag.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Represents a tag in TMX file.
    /// </summary>
    public struct TmxTag
    {
        /// <summary>
        /// TagType of the TMX tag.
        /// </summary>
        public TmxTagType TmxTagType;

        /// <summary>
        /// Outer XML of the TMX tag.
        /// </summary>
        public string Value;
    }
}
