using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranslationAssistant.TranslationServices.Core;

namespace TranslationAssistant.Business
{
    public static class AllLanguageList
    {
        public static string AllLanguagesInAllLanguages = string.Empty;


        public static async Task<string> GetAllLanguages()
        {
            StringWriter writer = new StringWriter();
            Dictionary<string, string> languagelist = new Dictionary<string, string>();
            languagelist = await AvailableLanguages.GetLanguages();
            writer.WriteLine("{0}\t{1}\t{2}", "Language", "Language Code", "Display Name");
            writer.WriteLine("------------------------------------------------------");
            foreach (KeyValuePair<string, string> language in languagelist.ToList())
            {
                Dictionary<string, string> languagesinlanguage = await AvailableLanguages.GetLanguages(language.Key);
                foreach (KeyValuePair<string, string> lang in languagesinlanguage)
                {
                    writer.WriteLine("{0}\t{1}\t{2}", language.Key, lang.Key, lang.Value);
                }
                writer.WriteLine("\n\n");
            }

            return writer.ToString();
        }


    }
}
