using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{

    class Utterances : List<Utterances>
    {
        public List<Utterance> UtteranceList { get; set; }

        private List<Utterance> UtteranceResultList = new List<Utterance>();

        public Utterances(List<Utterance> utterances)
        {
            UtteranceList = utterances;
        }

        public Utterances()
        {

        }

        /// <summary>
        /// Distribute the New Text to the utterances that mirrors 
        /// the lengths of the original utterances.
        /// </summary>
        /// <param name="newtext">The new text.</param>
        /// <returns>The list of utterances containing the new text.</returns>
        public List<Utterance> Distribute(string newtext)
        {

            //Sum the lengths of each original utterance
            int sumlength = 0;
            foreach (Utterance u in UtteranceList)
            {
                sumlength += u.content.Length;
            }

            //Calculate the ratio of each utterance
            for (int i = 0; i < UtteranceList.Count; i++)
            {
                Utterance u = (Utterance)UtteranceList[i];
                if (u.content.Length < 1)
                {
                    u.portion = 0;
                    u.lines = 0;
                }
                else
                {
                    u.portion = (double) u.content.Length / (double) sumlength;
                    u.lines = CountNewlines(u.content) - 1;
                }

            }

            string remainingstring = newtext;

            //split the new text per ratio
            foreach (Utterance u in UtteranceList)
            {
                if (u.lines < 1)
                {

                }
                Utterance uResult = new Utterance(order: u.order, timecode: u.timecode, content: string.Empty);
                int targetlength = (int) (u.portion * sumlength);
                Debug.WriteLine("Targetlength: {0}", targetlength);

                string thistext = remainingstring.Substring(0 , FindClosestWordBreak(remainingstring, targetlength));
                string tempstring = remainingstring.Substring(0, 10);
                while (IsPunctuation(tempstring[0]))
                {
                    thistext += tempstring[0];
                    tempstring = tempstring.Substring(1);
                }
                uResult.content = thistext;
                remainingstring = remainingstring.Substring(uResult.content.Length);
                uResult.content = SplitLines(thistext, u.lines);
                UtteranceResultList.Add(uResult);
            }


            //redistribute to new utterance results

            return UtteranceResultList;
        }

        private string SplitLines(string thistext, int lines)
        {
            if (String.IsNullOrEmpty(thistext) || (lines < 1)) return string.Empty;
            if (lines == 1) return thistext;
            StringBuilder result = new StringBuilder();
            int avgLength = (int) (result.Length / lines);
            string remainingtext = thistext;
            for (int i=0; i<lines; i++)
            {
                int endindex = FindClosestWordBreak(remainingtext, avgLength);
                string interim = remainingtext.Substring(0, endindex);
                interim = interim .Replace("\r\n", " ");
                interim = interim.Replace("\n", " ");
                interim = interim.Replace("\n", " ");
                result.AppendLine(interim);
                remainingtext = remainingtext.Substring(endindex);
                Debug.WriteLine("SplitLines.remainingtext: {0}", remainingtext);
            }

            return result.ToString();
        }

        private int FindClosestWordBreak(string input, int targetlength)
        {
            Random random = new Random();
            if ((input.Length <= (targetlength + 2)) || (input.Length <=2) || (input.Length <= (targetlength - 2))) return input.Length;
            if (IsBreakCharacter(input[targetlength])) return targetlength;
            for (int i=1; i<targetlength; i++)
            {
                if ((targetlength + i) >= input.Length) return targetlength + i;
                if (IsBreakCharacter(input[targetlength + i])) return targetlength + i;
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




        /// <summary>
        /// Calculates the number of newlines in a string.
        /// </summary>
        /// <param name="content">Text Content to count</param>
        /// <returns>Number of lines</returns>
        private int CountNewlines(string content)
        {
            if (content.Length < 1) return 0;
            int count = 1;
            foreach (char c in content)
            {
                if (c == '\n') count++;
            }
            return count;
        }
    }
}
