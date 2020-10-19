using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{
    /// <summary>
    /// Translate VTT and SRT caption files.
    /// The function this class performs is to combine utterances into sententes.
    /// then translate as whole sentences, and then distribute to the original time code
    /// with a line length that approximates the original closely.
    /// </summary>
    class VTTTranslate
    {
        #region Public Properties
        #endregion Public Properties

        #region Private Properties
        private List<string> Header = new List<string>();
        private List<Utterance> utterances = new List<Utterance>();
        private string filename = null;
        public string langcode = null;
        public enum Filetype { srt, vtt };
        public Filetype filetype = Filetype.vtt;

        #endregion Private Properties

        #region Methods

        public VTTTranslate(string filename, string langcode = "Detect", Filetype filetype = Filetype.vtt)
        {
            this.filename = filename;
            this.langcode = langcode;
            this.filetype = filetype;
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
                    if (filetype == Filetype.srt)
                    {
                        //read the utterance number
                        try
                        {
                            uttindex = System.Convert.ToInt16(line.Trim());
                            if (uttindex > 0) continue;
                        }
                        catch (System.FormatException) { }
                    }


                    if (line.Trim().Length > 0 && char.IsDigit(line.Trim()[0]) && line.Contains("-->"))
                    {
                        //this is a time code line.
                        Utterance u = new Utterance(uttindex, string.Empty, string.Empty);
                        uttindex++;
                        u.Timecode = line;
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
                                utterances[utterances.Count - 1].Content += line + " ";
                                utterances[utterances.Count - 1].Lines++;
                            }
                            else
                            {
                                line = ReplaceLanguageCode.Replace(line, langcode, tolangcode);
                                Header.Add(line);
                            }
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
                    if (filetype == Filetype.srt) newVTT.WriteLine(u.Order);
                    newVTT.WriteLine(u.Timecode);
                    newVTT.WriteLine(u.Content);
                    newVTT.WriteLine();
                }
                newVTT.Close();
            }

            return 0;
        }


        #endregion Methods
    }
}
