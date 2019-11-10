using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TranslationAssistant.TranslationServices.Core;

namespace TranslationAssistant.Business
{
    public class TranslateToAll
    {
        public event EventHandler OneTranslationDone;

        private async Task<SortedDictionary<string, string>> TranslateToAllLanguages(string text, string from, string category, TranslationServiceFacade.ContentType contentType)
        {
            EventHandler handler = OneTranslationDone;
            SortedDictionary<string, string> translatedDictionary = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> language in TranslationServiceFacade.AvailableLanguages)
            {
                string translation = await TranslationServiceFacade.TranslateStringAsync(text, from, language.Key, category, contentType);
                translatedDictionary.Add(language.Key, translation);
                handler(this, EventArgs.Empty);
            };
            return translatedDictionary;
        }

        

        public async Task<string> TranslateToAllLanguagesString(string text, string from, string category, TranslationServiceFacade.ContentType contentType)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                _ = new SortedDictionary<string, string>();
                SortedDictionary<string, string> translatedDictionary = await TranslateToAllLanguages(text, from, category, contentType);
                foreach (string key in translatedDictionary.Keys)
                {
                    string value = string.Empty;
                    try
                    {
                        translatedDictionary.TryGetValue(key, out value);
                    }
                    catch { };
                    stringWriter.WriteLine(key + ":\t" + value);
                }

                return stringWriter.ToString();
            }
        }
    }
}
