using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MTLWB.Common
{
    /// <summary>
    /// Represents which TUV tag is to be extracted for translation
    /// </summary>
    public enum TmxProcessingDirection
    {
        /// <summary>
        /// Extract the source TUV sentences for translation and write the translated senteces to target TUV tag.
        /// </summary>
        SourceToTarget,
        /// <summary>
        /// Extract the target TUV sentences for translation and write the translated senteces back to the same target TUV tag.
        /// </summary>
        TargetToTarget
    }

    /// <summary>
    /// Provides static methods used across the application
    /// </summary>
    public class Statics
    {
        /// <summary>
        /// Size of the SNT chunk
        /// </summary>
        public static readonly int CHUNK_SIZE = 5000;

        /// <summary>
        /// Performs a series of required regex replacements (for the characters like 'amp;', 'lt;', 'gt;') on specified text before sending text for MT.
        /// </summary>
        /// <param name="text">text on which regex replacements are to be performed</param>
        /// <returns>text after regex replacements</returns>
        public static string ReplaceBeforeMT(string text)
        {
            text = Regex.Replace(text, @"(&\s*?amp;lt)", "&amplt");
            text = Regex.Replace(text, @"(&\s*?amp;gt)", "&ampgt");
            text = Regex.Replace(text, @"(\s{1,}>)", ">");
            text = Regex.Replace(text, @"(<\s{1,})", "<");
            text = text.Replace(".<", ". <");
            return text;
        }

        /// <summary>
        /// Performs a series of required regex replacements (for the characters like 'amp;', 'lt;', 'gt;') on specified text after the text is MTed.
        /// </summary>
        /// <param name="text">text on which regex replacements are to be performed</param>
        /// <returns>text after regex replacements</returns>
        public static string ReplaceAfterMT(string text)
        {
            text = Regex.Replace(text, @"(&\s*?LT)", "&lt");
            text = Regex.Replace(text, @"(&\s*?GT)", "&gt");
            text = text.Replace("&<", "&amp;<");
            text = Regex.Replace(text, @"(&\s*?amplt\s*?&\s*?[^(ampltgt)])", "&amp;lt");
            text = Regex.Replace(text, @"(&\s*?amplt)", "&amp;lt");
            text = Regex.Replace(text, @"(&\s*?ampgt\s*?&\s*?[^(ampltgt)])", "&amp;gt");
            text = Regex.Replace(text, @"(&\s*?ampgt)", "&amp;gt");
            text = text.Replace("&)", "&amp;)");
            text = Regex.Replace(text, @"(&\s{1,}[^(ampLTlt)])", "&amp; ");
            text = text.Replace("&.", "&amp;.");
            text = text.Replace("&,", "&amp;,");
            text = text.Replace("&;", "&amp;;");
            text = text.Replace("&&", "&amp;&");
            return text;
        }

    }
}
