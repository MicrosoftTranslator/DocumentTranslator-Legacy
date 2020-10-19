using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{

    class Utterances : List<Utterances>
    {
        public List<Utterance> UtteranceList { get; set; }
        public string langcode { get; set; }

        private List<Utterance> UtteranceResultList = new List<Utterance>();

        private List<int> groupsumlengths = new List<int>();

        public Utterances(List<Utterance> utterances)
        {
            UtteranceList = utterances;
        }

        public Utterances()
        {

        }

        public Utterances(string languagecode)
        {
            langcode = languagecode;
        }

        /// <summary>
        /// Translate a list of utterances and distribute the translation in equal length. 
        /// </summary>
        /// <param name="tolang">TO language</param>
        /// <returns>List of translated utterances</returns>
        public async Task<List<Utterance>> Translate(string tolang)
        {
            List<string> listtotranslate = new List<string>();
            int groupid = 0;
            string stringtotranslate = string.Empty;
            foreach (Utterance utt in UtteranceList)
            {
                utt.Group = groupid;
                stringtotranslate += utt.Content.Trim() + " ";
                if (IsSentEndPunctuation(LastNonWhiteCharacter(stringtotranslate)) ||
                    utt.Content.Trim().Length < 1 ||
                    utt.Lines == 0)
                {
                    listtotranslate.Add(stringtotranslate);
                    groupid++;
                    groupsumlengths.Add(stringtotranslate.Length);
                    stringtotranslate = string.Empty;
                }
            }
            List<string> resultlist = await new TranslateList().Translate(listtotranslate, langcode, tolang);
            Debug.Assert(groupid == resultlist.Count);
            return Distribute(resultlist);
        }

        private char LastNonWhiteCharacter(string text)
        {
            if (text.Length < 1) return char.MinValue;
            string candidate = text.Trim();
            if (candidate.Length < 1) return char.MinValue;
            return candidate[candidate.Length - 1];
        }


        /// <summary>
        /// Distribute the New Text to the utterances that mirrors 
        /// the lengths of the original utterances.
        /// </summary>
        /// <param name="newtext">The new text.</param>
        /// <returns>The list of utterances containing the new text.</returns>
        private List<Utterance> Distribute(List<string> newtext)
        {
            //Calculate the ratio of each utterance per group. 
            for (int utteranceindex = 0; utteranceindex < UtteranceList.Count; utteranceindex++)
            {
                if ((UtteranceList[utteranceindex].Content.Length < 1) || (groupsumlengths[UtteranceList[utteranceindex].Group] == 0))
                {
                    UtteranceList[utteranceindex].Portion = 0;
                    UtteranceList[utteranceindex].Lines = 0;
                }
                else
                {
                    UtteranceList[utteranceindex].Portion = (double)UtteranceList[utteranceindex].Content.Length / groupsumlengths[UtteranceList[utteranceindex].Group];
                }
            }

            string remainingstring = string.Empty;

            //split the new text per ratio
            for (int groupindex = 0; groupindex < newtext.Count; groupindex++)
            {
                //get all utterances for this group
                IEnumerable<Utterance> grouplist = UtteranceList.Where(utterance => utterance.Group == groupindex);

                List<double> portions = new List<double>();
                foreach (var item in grouplist)
                {
                    portions.Add(item.Portion);
                }
                List<string> splitlist = SplitString(newtext[groupindex], portions);
                List<Utterance> newgrouplist = grouplist.ToList();
                for (int utteranceindex = 0; utteranceindex < grouplist.Count(); utteranceindex++)
                {
                    Utterance utt = newgrouplist[utteranceindex];
                    utt.Content = splitlist[utteranceindex];
                    utt.Content = SplitLines(utt.Content, utt.Lines);
                    UtteranceResultList.Add(utt);
                }
            }
            return UtteranceResultList;
        }

        /// <summary>
        /// Splits a text into portions of uneven length
        /// </summary>
        /// <param name="text">The text to split</param>
        /// <param name="portions">The distribution of portions</param>
        /// <returns>List of strings, each one corresponding in length to the portions key</returns>
        private List<string> SplitString(string text, List<double> portions)
        {
            List<double> normalizedportions = NormalizeDistribution(portions);
            List<string> resultlist = new List<string>(portions.Count);
            if (portions.Count <= 1)
            {
                resultlist.Add(text);
                return resultlist;
            }
            int firstbreak = FindClosestWordBreak(text, (int)(normalizedportions[0] * text.Length));
            resultlist.Add(text.Substring(0, firstbreak));
            string remainder = text.Substring(firstbreak);
            List<double> newportions = new List<double>(portions.Count);
            newportions = portions;
            newportions.RemoveAt(0);
            resultlist.AddRange(SplitString(remainder, newportions));
            return resultlist;
        }


        /// <summary>
        /// Normalize a distribution to sum up to 1.
        /// </summary>
        /// <param name="distribution">Distribution key</param>
        /// <returns>Normalized distribution key, sums to 1</returns>
        private static List<double> NormalizeDistribution(List<double> distribution)
        {
            //Normalize the distribution
            double sumdistribution = 0;
            foreach (var dist in distribution) sumdistribution += dist;
            List<double> normalizeddistribution = new List<double>();
            foreach (var dist in distribution)
            {
                normalizeddistribution.Add(dist / sumdistribution);
            }
            return normalizeddistribution;
        }


        /// <summary>
        /// Inserts line breaks to make lines evenly long. Removes any pre-existing line breaks. 
        /// </summary>
        /// <param name="thistext">Original text</param>
        /// <param name="lines">Number of lines</param>
        /// <returns>Re-linebroken text</returns>
        private string SplitLines(string thistext, int lines)
        {
            if (string.IsNullOrEmpty(thistext) || (lines < 1)) return string.Empty;
            thistext = thistext.Replace("\r\n", " ");
            thistext = thistext.Replace("\r", " ");
            thistext = thistext.Replace("\n", " ");
            if (lines <= 1) return thistext.Trim();
            StringBuilder result = new StringBuilder();
            int avgLength = thistext.Length / lines;
            string remainingtext = thistext;
            for (int i = 0; i < (lines - 1); i++)
            {
                int endindex = FindClosestWordBreak(remainingtext, avgLength);
                string interim = remainingtext.Substring(0, endindex);
                result.AppendLine(interim.Trim());
                remainingtext = remainingtext.Substring(endindex);
            }
            result.Append(remainingtext.Trim());
            return result.ToString();
        }

        private int FindClosestWordBreak(string input, int targetlength)
        {
            if ((input.Length <= (targetlength + 2)) || (input.Length <= 2) || (input.Length <= (targetlength - 2))) return input.Length;
            if (IsPunctuation(input[targetlength - 1])) return targetlength;
            if (IsPunctuation(input[targetlength])) return targetlength + 1;
            if (IsPunctuation(input[targetlength + 1])) return targetlength + 2;
            for (int i = 1; i < targetlength; i++)
            {
                if ((targetlength + i) >= input.Length) return targetlength + i;
                if (IsBreakCharacter(input[targetlength + i]))
                {
                    if (input.Length > (targetlength + i + 2) && IsBreakCharacter(input[targetlength + i + 2])) return targetlength + i + 2;
                    if (IsBreakCharacter(input[targetlength + i + 1])) return targetlength + i + 1;
                    return targetlength + i;
                }
                if ((i < input.Length) && IsBreakCharacter(input[targetlength - i])) return targetlength - i;
            }
            return targetlength;
        }

        private bool IsBreakCharacter(char ch)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(ch);
            switch (category)
            {
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.SpaceSeparator:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsPunctuation(char ch)
        {
            if (ch == ',') return true;
            if (ch == '.') return true;
            if (ch == '?') return true;
            if (ch == '!') return true;
            if (ch == '-') return true;
            if (ch == ')') return true;
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(ch);
            switch (category)
            {
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.ParagraphSeparator:
                    return true;
                default:
                    return false;
            }
        }
        private bool IsSentEndPunctuation(char ch)
        {
            if (ch == '.') return true;
            if (ch == '?') return true;
            if (ch == '!') return true;
            if (ch == '#') return true;
            if (ch == ')') return true;
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(ch);
            switch (category)
            {
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                    return true;
                default:
                    return false;
            }
        }

    }
}
