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
        public string category;
        #endregion Properties

        #region Private Properties
        private List<string> Markup = new List<string>();
        private List<string> Content = new List<string>();
        private string filename = null;
        private string langcode = null;

        #endregion Properties

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
            
            using (StreamReader streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (Char.IsDigit(line.Trim()[0]) && line.Contains("-->"))
                    {
                        Markup.Add(line);
                    }
                    else
                    {
                        if (line.Trim().Length > 0) Content.Add(line);
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
                fromlangcode = await TranslationServiceFacade.DetectAsync(Content[Content.Count] + Content[Content.Count - 1] + Content[Content.Count - 2], true);
            }

            string result = await TranslationServiceFacade.TranslateStringAsync(sb.ToString(), fromlangcode, tolangcode, category);
            sb.Clear();

            //Compose the resulting VTT

            List<int> offsets = await TranslationServiceFacade.BreakSentencesAsync(result, tolangcode);
            List<string> resultVTT = new List<string>();
            resultVTT.Add(result.Substring(0, offsets[0]));
            for (int i=1; i <= offsets.Count; i++)
            {
                resultVTT.Add(result.Substring(offsets[i - 1], offsets[i]));
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
