// -
// <copyright file="RegEx.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// According to the sentence length threshold rules, this function will escape the characters from the string 
    /// using regular expressions.
    /// </summary>
    public static class RegEx
    {
        // If url is like (http:microsoft.com?linkid=1234)
        public const string Regex1 = @"((['(']https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";

        // If url is like )http:microsoft.com?linkid=1234)--Extreme case
        public const string Regex2 = @"(([')']https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";

        // If url is like http:microsoft.com?linkid=1234
        public const string Regex3 = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$~_?\+-=\\\.&]*)";

        // If url is like <a href="http://www.microsoft.com"></a>
        public const string Regex4 = @"[@#$%^&*()_+{}\/><~|]\s";

        // If sentence contains special character with spaces preceeding &  following i.e. hello * Microsoft
        public const string Regex5 = @"[@#$%^&*()_+{}\/><~|]\s";
        
        // If sentence contans characters like &nbsp;
        public const string Regex6 = @"&(\w*);";

        // If sentence contans characters like &nbsp or &gt
        public const string Regex7 = @"&\w+[@#$%^&*()_+{}\/><~|;.,\s]";

        // If sentence contans space special character and word then xclude it i.e. Hello &nbsp;
        public const string Regex8 = @"[&]\w*";

        private static List<Regex> items;

        static RegEx()
        {
            RegEx.items = new List<Regex>();
            items.Add(new Regex(Regex1, RegexOptions.Compiled));
            items.Add(new Regex(Regex2, RegexOptions.Compiled));
            items.Add(new Regex(Regex3, RegexOptions.Compiled));
            items.Add(new Regex(Regex4, RegexOptions.Compiled));
            items.Add(new Regex(Regex5, RegexOptions.Compiled));
            items.Add(new Regex(Regex6, RegexOptions.Compiled));
            items.Add(new Regex(Regex7, RegexOptions.Compiled));
            items.Add(new Regex(Regex8, RegexOptions.Compiled));
        }

        public static string EscapeCharacters(string text)
        {
            string trimmed = text;
            foreach (Regex regex in RegEx.items)
            {
                if (regex.IsMatch(trimmed))
                {
                    trimmed = regex.Replace(trimmed, string.Empty);
                }
            }

            return trimmed;
        }
    }
}
