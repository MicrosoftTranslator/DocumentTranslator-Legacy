/*
 * Translate SRT files (Movie Transcript files).
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
    /// SRT is a file format for captions.
    /// It is a text file format, with each utterance being described by:
    ///     Utterance number AS int
    ///     The time span as time --> time
    ///     One or more lines of the utterance
    ///     An empty line to end the utterance
    /// </summary>
    class SRTDocument
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
        const string nonspacelanguages = "zh, th, ja, ko, zh-hans, zh-hant, zh-chs, zh-cht";

        /// <summary>
        /// Hold one utterance of the SRT file
        /// </summary>
        private struct SrtUtterance
        {
            public int uttno;          //The SRT order number;
            public string timefromto;  //The string defining the from and to times with the arrow in between. Do not translate.
            public string utterance;   //The actual utterance. Concatenated from multile text lines into one, makes a better translation. 
            public int spanlines;      //Telling us how many lines this utterance had in the original. So we can reproduce later.
        }

        /// <summary>
        /// Hold all utterances of the SRT file
        /// </summary>
        private Dictionary<int, SrtUtterance> SrtContent = new Dictionary<int, SrtUtterance>();

        public SRTDocument()
        {

        }

        /// <summary>
        /// Load the SRT file into the internal document format, a dictionary.
        /// Key of the dictionary is the utterance number.
        /// </summary>
        /// <param name="srtdocument">A string containing the SRT document</param>
        /// <param name="langcode">The language code of the document</param>
        /// <returns>Count of utterances</returns>
        public int LoadSRT(string srtdocument, string langcode)
        {
            _langcode = langcode;
            string[] srt = srtdocument.Split('\n');
            int lineindex = 0;
            int utteranceindex = 0;
            while (lineindex < srt.Length)
            {
                srt[lineindex] = srt[lineindex].Trim();
                try
                {
                    utteranceindex = Convert.ToInt16(srt[lineindex]);
                }
                catch {
                    lineindex++;        // skip line and move on
                    continue;
                } 
                if (utteranceindex > 0)
                {
                    SrtUtterance srtutterance = new SrtUtterance();
                    srtutterance.uttno = utteranceindex;
                    lineindex++;
                    srtutterance.timefromto = srt[lineindex];
                    lineindex++;
                    srtutterance.utterance = string.Empty;
                    srtutterance.spanlines = 0;
                    while (srt[lineindex].Trim().Length > 0)
                    {
                        srtutterance.utterance += srt[lineindex];
                        //add a space between the lines for languages that use space
                        if (!nonspacelanguages.Contains(langcode.ToLowerInvariant())) srtutterance.utterance += " ";
                        lineindex++;
                        srtutterance.spanlines++;
                    }
                    srtutterance.utterance = srtutterance.utterance.Replace("\r", string.Empty);
                    SrtContent.Add(utteranceindex, srtutterance);
                }
            }
            return (SrtContent.Count);
        }

        public void Translate(string fromlanguage, string tolanguage)
        {
            Dictionary<int, SrtUtterance> ToDict = new Dictionary<int, SrtUtterance>();
            Parallel.ForEach(SrtContent, (srtutterance) =>
            {
                SrtUtterance toutterance = new SrtUtterance();
                toutterance = srtutterance.Value;
                toutterance.utterance = TranslationServices.Core.TranslationServiceFacade.TranslateString(srtutterance.Value.utterance, fromlanguage, tolanguage, 0);
                ToDict.Add(srtutterance.Key, toutterance);
            });
            _langcode = tolanguage;
            SrtContent = ToDict;        //Replace the master collection with the translated version. 
        }

        public override string ToString()
        {
            StringWriter tosrt = new StringWriter();
            foreach (var srtutterance in SrtContent.OrderBy(Key => Key.Key))
            {
                tosrt.WriteLine(srtutterance.Key);
                tosrt.Write(srtutterance.Value.timefromto);
                tosrt.Write(splitevenly(srtutterance.Value.utterance, srtutterance.Value.spanlines, _langcode));
                tosrt.WriteLine();          //end utterance with an empty line
            }
            return tosrt.ToString();
        }

        private string splitevenly(string utterance, int segments, string langcode)
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
                    result.WriteLine(utterance.Substring(startindex, endindex));
                    startindex = endindex + 1;
                }
                result.WriteLine(utterance.Substring(startindex));          //copy the last segment
            }
            string debug = result.ToString();
            return result.ToString();
        }
    }


    class SRTTranslationManager
    {
        public static int DoTranslation(string srtfilename, string fromlanguage, string tolanguage)
        {
            string srttext = File.ReadAllText(srtfilename);
            SRTDocument srtdocument = new SRTDocument();
            int noofutterances = srtdocument.LoadSRT(srttext, fromlanguage);
            srtdocument.Translate(fromlanguage, tolanguage);
            File.WriteAllText(srtfilename, srtdocument.ToString(), Encoding.UTF8);
            return noofutterances;
        }
    }
}
