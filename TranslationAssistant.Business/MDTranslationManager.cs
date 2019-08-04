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

        private enum LineType { star, dash, equalsign, code, translatable };

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
            while (lineindex < md.Length)
            {
                paragraphindex++;
                if (paragraphindex > 0)
                {
                    MDUtterance mdutterance = new MDUtterance
                    {
                        text = string.Empty,
                        spanlines = 0,
                        lineType = LineType.translatable
                    };
                    while (md[lineindex].Trim().Length > 0)
                    {
                        mdutterance.text += md[lineindex];
                        //add a space between the lines for languages that use space
                        if (!nonspacelanguages.Contains(langcode.ToLowerInvariant())) mdutterance.text += " ";
                        lineindex++;
                        mdutterance.spanlines++;
                        if (lineindex == md.Length) break;
                    }
                    lineindex++;
                    mdutterance.text = mdutterance.text.Replace("\r", string.Empty);
                    MDContent.Add(paragraphindex, mdutterance);
                }
            }
            return MDContent.Count;
        }

        public void Translate(string fromlanguage, string tolanguage)
        {
            Dictionary<int, MDUtterance> ToDict = new Dictionary<int, MDUtterance>();
            Parallel.ForEach(MDContent, (mdutterance) =>
            {
                MDUtterance toutterance = new MDUtterance();
                toutterance = mdutterance.Value;
                toutterance.text = TranslationServices.Core.TranslationServiceFacade.TranslateString(mdutterance.Value.text, fromlanguage, tolanguage, 0);
                ToDict.Add(mdutterance.Key, toutterance);
            });
            _langcode = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(tolanguage);
            MDContent = ToDict;        //Replace the master collection with the translated version. 
        }

        public override string ToString()
        {
            StringWriter tomd = new StringWriter();
            foreach (var mdutterance in MDContent.OrderBy(Key => Key.Key))
            {
                tomd.Write(Splitevenly(mdutterance.Value.text, mdutterance.Value.spanlines, _langcode));
                tomd.WriteLine();          //end utterance with an empty line
            }
            return tomd.ToString();
        }

        private string Splitevenly(string utterance, int segments, string langcode)
        {
            if (segments <= 1) return utterance + "\r\n";
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
                    result.WriteLine(utterance.Substring(startindex, endindex-startindex));
                    startindex = endindex + 1;
                }
                result.WriteLine(utterance.Substring(startindex));          //copy the last segment
            }
            string debug = result.ToString();
            return result.ToString();
        }
    }


    class MDTranslationManager
    {
        public static int DoTranslation(string mdfilename, string fromlanguage, string tolanguage)
        {
            string mdtext = File.ReadAllText(mdfilename);
            MDDocument mddocument = new MDDocument();
            string _fromlanguagecode = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(fromlanguage);
            int noofutterances = mddocument.LoadMD(mdtext, _fromlanguagecode);
            mddocument.Translate(fromlanguage, tolanguage);
            File.WriteAllText(mdfilename, mddocument.ToString(), Encoding.UTF8);
            return noofutterances;
        }
    }
}
