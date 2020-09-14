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
        private List<string> Markup = new List<string>();
        private List<string> Content = new List<string>();
        private List<string> Header = new List<string>();
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
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Trim().Length>0 && Char.IsDigit(line.Trim()[0]) && line.Contains("-->"))
                    {
                        Markup.Add(line);
                        headerended = true;
                    }
                    else
                    {
                        if (line.Trim().Length > 0)
                        {
                            if (headerended) Content.Add(line);
                            else Header.Add(line);
                        }
                    }
                }
            }

            //Concatenate the string
            StringBuilder sb = new StringBuilder();
            foreach (var line in Content)
            {
                sb.Append(line + " ");
            }

            //Translate

            string fromlangcode = null;
            if (Content.Count > 3)
            {
                string sample = Content[Content.Count/2] + Content[Content.Count/2 - 1] + Content[Content.Count/2 + 1];
                fromlangcode = await TranslationServiceFacade.DetectAsync(sample, true);
            }

            string result = await TranslationServiceFacade.TranslateStringAsync(sb.ToString(), fromlangcode, tolangcode);
            sb.Clear();

            //Compose the resulting VTT

            List<int> offsets = await TranslationServiceFacade.BreakSentencesAsync(result, tolangcode);
            List<string> resultVTT = new List<string>();
            resultVTT.AddRange(Header);
            int startindex = 0;
            for (int i=0; i < offsets.Count; i++)
            {
                resultVTT.Add(result.Substring(startindex, offsets[i]));
                startindex += offsets[i];
            }

            using (StreamWriter outVTT = new StreamWriter(filename))
            {
                foreach (string line in resultVTT) outVTT.WriteLine(line);
            }
            return resultVTT.Count;
        }

        #endregion Methods
    }
}
