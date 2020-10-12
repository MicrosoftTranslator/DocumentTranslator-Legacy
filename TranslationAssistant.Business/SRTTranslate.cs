using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{
    class SRTTranslate
    {
        #region Public Properties
        #endregion Public Properties

        #region Private Properties
        private List<string> Header = new List<string>();
        private List<Utterance> utterances = new List<Utterance>();
        private string filename = null;
        public string langcode = null;

        #endregion Private Properties

        #region Methods

        public SRTTranslate(string filename, string langcode = "Detect")
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
                int uttindex = 0;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Trim().Length > 0 && char.IsDigit(line.Trim()[0]) && line.Contains("-->"))
                    {
                        //this is a time code line.
                        Utterance u = new Utterance(uttindex, string.Empty, string.Empty);
                        uttindex++;
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

            //Translate the utterances
            Utterances utt = new Utterances(utterances);
            List<Utterance> newutt = await utt.Translate(tolangcode);


            //Write out the target file
            using (StreamWriter newVTT = new StreamWriter(filename))
            {
                foreach (var line in Header)
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


        #endregion Methods
    }
}
