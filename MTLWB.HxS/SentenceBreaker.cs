using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;

namespace MTLWB.HxS
{
    /// <summary>
    /// Summary description for SentenceBreaker
    /// </summary>
    internal class SentenceBreaker
    {
        private static string _terminators = "!.?。"; // sentence terminators

        //changed by Tejas Desai on Aug 20, 2009
        private static string _terminator_followers = " \n\r\t";
        //private static string _terminator_followers = " \n\r\t%n"; // % is for %n etc.
        
        private static string _hardterminators = "。"; // hard sentence terminators (not followed by space)
        private static string _xxfixnoncare = "\n\r\t\"' "; // == all the stuff that belongs into pre/suffixes

        public static LinkedList<string> Break(string strTextChunk, CultureInfo ci)
        {

            // a sentence ends with one of the characters in _terminators
            // any characters that follow the end of the sentence and are included in the
            // 'noncare' characters are appended to the sentence


            LinkedList<string> res = new LinkedList<string>();
            int startpos = 0, termpos = 0, len, segment = 0;
            len = strTextChunk.Length;

            while (startpos < len)
            {
                segment++;
                termpos = -1;
                int tpos;
                string bold = "";
                for (int i = 0; i < _terminators.Length; i++)
                {
                    int offset = startpos; // examine all _terminators[i] within the rest of the string
                    while ((tpos = strTextChunk.IndexOf("" + _terminators[i], offset, len - offset)) != -1)
                    {
                        offset = tpos + 1;
                        if ((tpos < termpos) || (termpos == -1))
                        {
                            // it is a real terminator only if it is followed by
                            // a blank char (one of the _terminator_followers)
                            // or if it is the last char in the sentence
                            // special case for \\ which might be the start of some stupid rtf command (eg \\par)
                            // or if the terminator was a hard terminator (eg chinese .)
                            if ((tpos + 1 == len) || (_terminator_followers.IndexOf(strTextChunk[tpos + 1]) != -1)
                                || (strTextChunk[tpos + 1] == '\\') || (_hardterminators.IndexOf(strTextChunk[tpos]) != -1))
                            {
                                // If the terminiator is in the middle of a tag that opens and closes within the segment 
                                // (for example, a <link> tag with a title that includes a period in it), then
                                // don't break the sentence.
                                if (!(TerminatorIsMidSentence(strTextChunk, tpos)))
                                    termpos = tpos;
                            }
                        }
                    }
                }


                if (termpos != -1)
                {
                    while ((termpos + 1 < len) && (_xxfixnoncare.IndexOf(strTextChunk[termpos + 1]) != -1))
                        termpos++;
                    res.AddLast(strTextChunk.Substring(startpos, termpos - startpos + 1));
                    startpos = termpos + 1 + bold.Length;
                }
                else
                // no more sentences to break down
                {
                    if (startpos <= len - 1)
                        res.AddLast(strTextChunk.Substring(startpos, len - startpos));
                    startpos = len;
                }
            }

            return res;
        }
        /// <summary>
        /// Determines whether the terminator at the specified position in the string is within a tag 
        /// within the chunk that should not be broken.  This can happen, for example, when a link tag includes
        /// a topic title that contains a period.  In cases like this, the chunk should not be broken into 
        /// multiple sentences.
        /// </summary>
        /// <param name="chunk">The string to analyze.</param>
        /// <param name="terminatorPosition">The position of the terminator in question within the string.</param>
        /// <returns>tru to indicate that the terminator is within a block of tags that should not be broken apart.</returns>
        private static bool TerminatorIsMidSentence(string chunk, int terminatorPosition)
        {
            // Determine whether the chunk contains any pairs of opening and closing tags.
            //int i = 0;
            string tempChunk = chunk;
           // while (i < chunk.Length && i < terminatorPosition)
            while (tempChunk.Length > terminatorPosition)
            { 
                int startOfTag = tempChunk.IndexOf("<");
                if (startOfTag == -1 || startOfTag > terminatorPosition)
                    break;
                int endOfTag = tempChunk.IndexOf(">");
                int checkForSpace = tempChunk.IndexOf(" ", startOfTag);
                // If checkForSpace != -1 and checkForSpace < endOfTag, that means the tag contains 
                // attributes.  We just want the tag name, not the attribute info, so use checkForSpace
                // rather than endOfTag to get the tag name.  
                string tagName;
                if (checkForSpace != -1 && checkForSpace < endOfTag)
                    tagName = tempChunk.Substring(startOfTag + 1, (checkForSpace - startOfTag) - 1);
                else
                    tagName = tempChunk.Substring(startOfTag + 1, (endOfTag - startOfTag) - 1);
                
                int startOfClosingTag = tempChunk.IndexOf("</" + tagName + ">");

                // If yes, determine whether the terminator at the specified position is between the tags.
                if (terminatorPosition > endOfTag && terminatorPosition < startOfClosingTag)
                    return  true;

                // reset i and tempChunk and continue to look for more pairs of tags.
                //i = endOfTag + 1;
                tempChunk = tempChunk.Substring(endOfTag + 1);
                terminatorPosition = terminatorPosition - (endOfTag+1);

            }
            return false;
        }
    }
}