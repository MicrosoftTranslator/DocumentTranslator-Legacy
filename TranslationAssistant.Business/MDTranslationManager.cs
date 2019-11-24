/*
 * Translate Markdown files (.MD files).
 * We are trying to follow the Github markdown specification
 * https://guides.github.com/features/mastering-markdown/
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TranslationAssistant.Business
{
    /// <summary>
    /// MD is a file format for markdown.
    /// </summary>
    class MDDocument
    {
        /// <summary>
        /// Hold the information which language this document is in.
        /// </summary>
        private string _langcode;
        public string langcode
        {
            get { return _langcode; }
            set { string _langcode = langcode; }
        }

        /// <summary>
        /// List of languages that do not use space as word saparator.
        /// Must be listed in all lowercase.
        /// </summary>
        private const string nonspacelanguages = "zh, th, ja, ko, zh-hans, zh-hant, zh-chs, zh-cht";

        private enum LineType { empty, title, code, translatable, blockquote, list };

        /// <summary>
        /// Hold one utterance (aka paragraph) of the MD file
        /// </summary>
        private struct MDUtterance
        {
            public string text;        //Text of the utterance
            public int spanlines;      //Telling us how many lines this utterance had in the original. So we can reproduce later.
            public LineType lineType;  //Markup type for this utterance - decides whether it needs translation
        }

        /// <summary>
        /// Hold all utterances of the MD file.
        /// The index is the paragraph number in the MD document.
        /// </summary>
        private Dictionary<int, MDUtterance> MDContent = new Dictionary<int, MDUtterance>();

        public MDDocument()
        {

        }

        /// <summary>
        /// Load the MD file into the internal document format, a dictionary.
        /// Key of the dictionary is the utterance number.
        /// </summary>
        /// <param name="mddocument">A string containing the MD document</param>
        /// <param name="langcode">The language code of the document</param>
        /// <returns>Count of utterances</returns>
        public int LoadMD(string mddocument, string langcode)
        {
            _langcode = langcode;
            string[] md = mddocument.Split('\n');
            int lineindex = 0;
            int paragraphindex = 0;
            bool insidecodesection = false;
            while (lineindex < md.Length)
            {
                string trimline = md[lineindex].Trim();
                paragraphindex++;
                if (paragraphindex > 0)
                {
                    MDUtterance mdutterance = new MDUtterance
                    {
                        text = string.Empty,
                        spanlines = 0,
                        lineType = LineType.translatable
                    };

                    //Check for empty string
                    if (trimline.Length == 0)
                    {
                        mdutterance.lineType = LineType.empty;
                        MDContent.Add(paragraphindex, mdutterance);
                        lineindex++;
                        continue;
                    }
                    //Mark lines enclosed with "'''" as untranslatable code. 
                    if (insidecodesection) mdutterance.lineType = LineType.code;
                    if (trimline.StartsWith(@"'''") || trimline.StartsWith(@"```"))
                    {
                        mdutterance.lineType = LineType.code;
                        if (insidecodesection)
                            insidecodesection = false;
                        else
                            insidecodesection = true;
                    }

                    //Check for Blockquote. Remove the > character and treat as continuous utterance. Mark utterance as linetype.blockquote
                    if (trimline.StartsWith(@">"))
                    {
                        trimline = trimline.Substring(1).Trim();
                        mdutterance.lineType = LineType.blockquote;
                    }

                    //Check for Lists
                    if (trimline.StartsWith(@"*"))
                    {
                        mdutterance.lineType = LineType.list;
                    }

                    //Check for heading. If heading, leave as single paragraph, do not combine lines. Else combine lines until an empty line.
                    if (trimline.Length > 0)
                    {
                        if (trimline.StartsWith("#") || insidecodesection || trimline.StartsWith(@"'''") || trimline.StartsWith(@"```") || trimline.StartsWith(@"*") || trimline.StartsWith(@"-"))
                        {
                            //create a single-line utterance
                            mdutterance.text = trimline;
                        }
                        else
                        {
                            //combine multiple lines to a single utterance
                            while (md[lineindex].Trim().Length > 0)
                            {
                                //break on ''' (start of untranslatable code section)
                                if (md[lineindex].TrimStart().StartsWith(@"'''") || md[lineindex].TrimStart().StartsWith(@"```"))
                                {
                                    MDContent.Add(paragraphindex, mdutterance);
                                    insidecodesection = true;
                                    mdutterance.text = md[lineindex];
                                    mdutterance.lineType = LineType.code;
                                    mdutterance.spanlines = 1;
                                    paragraphindex++;
                                    break;
                                }

                                //check for blockquote
                                if (md[lineindex].TrimStart().StartsWith(@">"))
                                {
                                    mdutterance.lineType = LineType.blockquote;
                                    mdutterance.text += md[lineindex].Trim().Substring(1);
                                }
                                else
                                {
                                    mdutterance.text += md[lineindex].Trim();
                                }
                                //add a space between the lines for languages that use space
                                if (!nonspacelanguages.Contains(langcode.ToLowerInvariant())) mdutterance.text += " ";
                                lineindex++;

                                if (lineindex == md.Length) break;
                                mdutterance.spanlines++;
                            }
                        }
                        mdutterance.text = mdutterance.text.Replace("\r\n", string.Empty);
                    }
                    Debug.WriteLine(mdutterance.lineType.ToString() + ": " + mdutterance.text);
                    MDContent.Add(paragraphindex, mdutterance);
                    lineindex++;
                }
            }
            return MDContent.Count;
        }

        public void Translate(string fromlanguage, string tolanguage)
        {
            Dictionary<int, MDUtterance> ToDict = new Dictionary<int, MDUtterance>();
//            Parallel.ForEach(MDContent, (mdutterance) =>
            foreach(var mdutterance in MDContent)
            {
                MDUtterance toutterance = new MDUtterance();
                toutterance = mdutterance.Value;
                if (mdutterance.Value.lineType != LineType.code)
                {
                    toutterance.text = Tagged2Markdown(TranslationServices.Core.TranslationServiceFacade.TranslateString(Markdown2Tagged(mdutterance.Value.text), fromlanguage, tolanguage, 0));
                }
                ToDict.Add(mdutterance.Key, toutterance);
            };
            _langcode = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(tolanguage);
            MDContent = ToDict;        //Replace the master collection with the translated version. 
        }

        public override string ToString()
        {
            StringWriter tomd = new StringWriter();
            foreach (var mdutterance in MDContent.OrderBy(Key => Key.Key))
            {
                Debug.WriteLine(mdutterance.Value.lineType.ToString() + ": " + mdutterance.Value.text);
                if (mdutterance.Value.lineType == LineType.blockquote)
                {
                    StringWriter blockquote = new StringWriter();
                    blockquote.Write(Splitevenly(mdutterance.Value.text, mdutterance.Value.spanlines, _langcode));
                    tomd.WriteLine(">" + blockquote.ToString().Replace("\r\n", "\r\n> "));
                }
                else
                {
                    string splitstring = string.Empty;
                    if (mdutterance.Value.lineType == LineType.translatable)
                        splitstring = Splitevenly(mdutterance.Value.text, mdutterance.Value.spanlines, _langcode);
                    else
                        splitstring = mdutterance.Value.text.TrimEnd();
                    tomd.WriteLine(splitstring);
                }
            }
            return tomd.ToString();
        }

        private string Splitevenly(string utterance, int segments, string langcode)
        {
            if (segments <= 1) return utterance;
            StringWriter result = new StringWriter();
            int segmentlength = utterance.Length / segments;
            if (nonspacelanguages.Contains(langcode.ToLowerInvariant()))    //non-spacing languages
            {
                for (int i = 0; i < segments; i++) result.WriteLine(utterance.Substring(segmentlength * i, segmentlength));
            }
            else                                                            //spacing languages
            {
                int startindex = 0; 
                for (int i = 1; i < segments; i++)
                {
                    int endindex = utterance.IndexOf(' ', segmentlength * i);
                    if (endindex > startindex) result.WriteLine(utterance.Substring(startindex, endindex-startindex));
                    startindex = endindex + 1;
                }
                result.WriteLine(utterance.Substring(startindex));          //copy the last segment
            }
            string debug = result.ToString();
            return result.ToString();
        }

        private struct MDTag
        {
            public string value;    //value
        }

        private Dictionary<string, MDTag> MDTags = new Dictionary<string, MDTag>();

        private int g_count = 0;   //global counter for tag numbering


        /// <summary>
        /// Replace markdown with <C0> style tags
        /// Assumes a single utterance. Do not pass in multi-paragraph strings. 
        /// Writes to the global MDTags dictionary
        /// </summary>
        /// <param name="input">Markdown string</param>
        /// <returns>Same string with markdown replaced by <C0> style tags</returns>
        private string Markdown2Tagged(string input)
        {
            g_count = 0;
            MDTags.Clear();
            bool inside = false;
            string output = string.Empty;
            int startofcopy = 0;
            MatchCollection matchCollection = Regex.Matches(input, @"(#+)|(\*+)|(~~+)|(_+)|(')");

            foreach (Match match in matchCollection)
            {
                if (!inside) g_count++;
                string key = inside ? "/a" : "a";
                if (match.Value[0] == '\'')
                    key = inside ? "/c" : "c";
                key += g_count;
                inside = ((match.Value[0] == '*') || match.Value[0] == '~' || match.Value[0] == '_' || match.Value[0] == '\'') && !inside ? true : false;
                MDTag mdtag = new MDTag
                {
                    value = match.Value
                };
                MDTags.Add(key, mdtag);
                output += input.Substring(startofcopy, match.Index-startofcopy) + "<" + key + ">";
                startofcopy = match.Index + match.Length;
            }
            output += input.Substring(startofcopy);
            return output;
        }


        private string Tagged2Markdown(string input)
        {
            string output = input;
            foreach(var mdtag in MDTags)
            {
                output = output.Replace("<"+mdtag.Key+">", mdtag.Value.value);
            }
            return output;
        }
    }


    class MDTranslationManager
    {
        public static async Task<int> DoTranslation(string mdfilename, string fromlanguage, string tolanguage)
        {
            string mdtext = File.ReadAllText(mdfilename);
            MDDocument mddocument = new MDDocument();
            string _fromlanguagecode = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(fromlanguage);
            if (_fromlanguagecode.Length < 1) _fromlanguagecode = await TranslationServices.Core.TranslationServiceFacade.DetectAsync(mdtext, true);
            int noofutterances = mddocument.LoadMD(mdtext, _fromlanguagecode);
            mddocument.Translate(fromlanguage, tolanguage);
            File.WriteAllText(mdfilename, mddocument.ToString(), Encoding.UTF8);
            return noofutterances;
        }
    }
}
