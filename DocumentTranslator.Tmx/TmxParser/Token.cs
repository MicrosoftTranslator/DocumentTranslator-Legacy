// -
// <copyright file="Token.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// Contains all the string constants used for TMX processing.
    /// </summary>
    public class Token
    {
        public const string SCRIPT = "script";
        public const string STYLE = "style";

        public const string LT = "<";
        public const string GT = ">";
        public const string SLGT = "/>";
        public const string LTSL = "</";
        public const string EQ = "=";
        public const string EX = "!";
        public const string SL = "/";
        public const string DQ = "\"";
        public const string SQ = "\'";
        public const string NL = "\n";
        public const string CR = "\r";
        public const string TAB = "\t";
        public const string CRNL = "\r\n";

        public const string XCS = "<!--";
        public const string XCE = "-->";

        public const string SGMLCS = "<!";
        public const string SGMLCE = ">";

        public const string EQDQ = "=\"";
        public const string SPACE = " ";
        public const string DSPACE = "  ";

        public const string UT = "ut";
        public const string PH = "ph";
        public const string BPT = "bpt";
        public const string EPT = "ept";
        public const string IT = "it";
        public const string TUV = "tuv";
        public const string SEG = "seg";
        public const string OB = "{";
        public const string CB = "}";

        public const string ASTART = "<A";
        public const string EEND = "/>";
        public const string TSTART = "<T";
        public const string TEND = "</T";

        public const string MTLWBSCRIPTLT = "[MTLWB-SCRIPT-LT]";
        public const string MTLWBSCRIPTGT = "[MTLWB-SCRIPT-GT]";
        public const string MTLWBSCRIPTCR = "[MTLWB-SCRIPT-CR]";
        public const string MTLWBSCRIPTLF = "[MTLWB-SCRIPT-LF]";

        public const string XMLOPEN = "<?XML ";
        public const string XMLCLOSE = "?>";
        public const string DOCTYPETAG = "<!DOCTYPE ";
        public const string TMXOPEN = "<TMX";
        public const string TMXCLOSE = "</TMX>";
        public const string HEADEROPEN = "<HEADER";
        public const string HEADERCLOSE = "</HEADER>";
        public const string BODYOPEN = "<BODY";
        public const string BODYCLOSE = "</BODY>";
        public const string TUOPEN = "<TU ";
        public const string TUOPEN1 = "<TU>";
        public const string TUOPEN2 = "<TU";
        public const string TUCLOSE = "</TU>";
    }
}
