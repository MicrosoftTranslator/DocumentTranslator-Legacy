// -
// <copyright file="ParseStatus.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// Internal FSM to represent the state of the parser.
    /// </summary>
    internal enum ParseStatus
    {
        ReadText = 0,
        ReadEndTag = 1,
        ReadStartTag = 2,
        ReadAttributeName = 3,
        ReadAttributeValue = 4
    }
}
