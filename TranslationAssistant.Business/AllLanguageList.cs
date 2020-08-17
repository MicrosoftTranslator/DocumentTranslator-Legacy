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
            Task GetLanguagesTask = TranslationServiceFacade.GetLanguages();
            StringWriter writer = new StringWriter();
            Dictionary<string, string> languagelist = new Dictionary<string, string>();
            await GetLanguagesTask;
            languagelist = TranslationServiceFacade.AvailableLanguages;
            writer.WriteLine("{0}\t{1}\t{2}", "Language", "Language Code", "Display Name");
            writer.WriteLine("------------------------------------------------------");
            foreach (KeyValuePair<string, string> language in languagelist.ToList())
            {
                Task t = TranslationServiceFacade.GetLanguages(language.Key);
                await t;
                foreach (KeyValuePair<string, string> lang in TranslationServiceFacade.AvailableLanguages)
                {
                    writer.WriteLine("{0}\t{1}\t{2}", language.Key, lang.Key, lang.Value);
                }
                writer.WriteLine("\n\n");
            }

            return writer.ToString();
        }


    }
}
