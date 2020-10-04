using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TranslationAssistant.TranslationServices.Core;

namespace TranslationAssistant.Business
{
    class VTTTranslate
    {
        #region Public Properties
        #endregion Public Properties

        #region Private Properties
        private List<string> Header = new List<string>();
        private List<Utterance> utterances = new List<Utterance>();
        private string filename = null;
        private string langcode = null;

        #endregion Private Properties

        #region Methods

        public VTTTranslate(string filename, string langcode = "Detect")
        {
            this.filename = filename;
            this.langcode = langcode;
        }

        /// <summary>
        /// Translate the VTT
        /// </summary>
        /// <param name="tolangcode">Translate to language</param>
        /// <returns>List of translated VTT</returns>
        public async Task<int> Translate(string tolangcode)
        {
            //Read into the Markup and Content arrays
            bool headerended = false;

            using (StreamReader streamReader = new StreamReader(filename))
            {
                int i = 0;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Trim().Length>0 && Char.IsDigit(line.Trim()[0]) && line.Contains("-->"))
                    {
                        //this is a time code line.
                        Utterance u = new Utterance(i, string.Empty, string.Empty);
                        u.timecode = line;
                        utterances.Add(u);
                        headerended = true;
                    }
                    else
                    {
                        if (line.Trim().Length > 0)
                        {
                            //this is a content line
                            if (headerended)
                            {
                                utterances[utterances.Count - 1].content += line + " ";
                                utterances[utterances.Count - 1].lines++;
                            }
                            else Header.Add(line);
                        }
                    }
                }
                streamReader.Close();
            }

            //Concatenate the string in groups.
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var u in utterances)
            {
                if (u.lines == 0)
                {
                    list.Add(sb.ToString());
                    sb.Clear();
                }

                sb.Append(u.content + " ");
            }
            if (sb.Length >= 1) list.Add(sb.ToString());
            sb.Clear();

            //Translate

            string fromlangcode = null;
            if (utterances.Count > 3)
            {
                string sample = utterances[utterances.Count/2].content + utterances[utterances.Count/2 - 1].content + utterances[utterances.Count/2 + 1].content;
                fromlangcode = await TranslationServiceFacade.DetectAsync(sample, true);
            }

            TranslateList translateList = new TranslateList();
            List<string> translationresult = await translateList.Translate(list, fromlangcode, tolangcode);

            //Compose the resulting VTT
            //translationresult = await InsertSentenceBreaks(translationresult, tolangcode);
            Utterances utt = new Utterances(utterances);
            List<Utterance> newutt = await utt.Distribute(translationresult);

            using (StreamWriter newVTT = new StreamWriter(filename))
            {
                foreach(var line in Header)
                {
                    newVTT.WriteLine(line);
                }
                newVTT.WriteLine();
                foreach (var u in newutt)
                {
                    newVTT.WriteLine(u.timecode);
                    newVTT.WriteLine(u.content);
                    newVTT.WriteLine();
                }
                newVTT.Close();
            }

            return 0;
        }

        private async Task<string> InsertSentenceBreaks(string input, string tolangcode)
        {
            List<int> sentbreaks = await TranslationServiceFacade.BreakSentencesAsync(input, tolangcode);
            StringBuilder sb = new StringBuilder();
            int startindex = 0;
            for (int i = 0; i < sentbreaks.Count; i++)
            {
                sb.AppendLine(input.Substring(startindex, sentbreaks[i]));
                startindex += sentbreaks[i];
            }
            return sb.ToString();
        }

        #endregion Methods
    }
}
