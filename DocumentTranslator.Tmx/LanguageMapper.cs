using System.Collections.Generic;
using System.IO;

namespace Mts.Common.Tmx
{
    public static class LanguageMapper
    {
        private const string CSVFileName = "TmxLangMap.csv";
        private static SortedDictionary<string, string> languagemap = new SortedDictionary<string, string>();

        /// <summary>
        /// Maps a language ID from one value to another value. Map is read from TmxLangMap.scv.
        /// </summary>
        /// <param name="langfrom">From language ID in the TMX</param>
        /// <returns>To language, for instance the Microsoft Translator language ID</returns>
        public static string MapLanguage(string langfrom)
        {
            string langto = langfrom;
            if (! languagemap.TryGetValue(langfrom, out langto)) langto = langfrom;
            return langto;
        }

        /// <summary>
        /// Static initializer function. Runs once.
        /// </summary>
        static LanguageMapper()
        {
            //SortedDictionary<string, string> languagemap = new SortedDictionary<string, string>();
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(CSVFileName);
            }
            catch { return; }

            foreach (string line in lines)
            {
                List<string> cells = new List<string>();
                cells = csvParser(line);
                try
                {
                    languagemap.Add(cells[0], cells[1]);
                }
                catch { }
            }
            return;
        }
        private static List<string> csvParser(string csv, char separator = ',')
        {
            List <string> parsed = new List <string>();
            string[] temp = csv.Split(separator);
            int counter = 0;
            string data = string.Empty;
            while (counter < temp.Length)
            {
                data = temp[counter].Trim();
                if (data.Trim().StartsWith("\""))
                {
                    bool isLast = false;
                    while (!isLast && counter < temp.Length)
                    {
                        data += separator.ToString() + temp[counter + 1];
                        counter++;
                        isLast = (temp[counter].Trim().EndsWith("\""));
                    }
                }
                parsed.Add(data);
                counter++;
            }

            return parsed;

        }
    }
}
