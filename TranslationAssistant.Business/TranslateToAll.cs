using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{
    public class TranslateToAll
    {

        public async Task<ConcurrentDictionary<string, string>> TranslateToAllLanguages(string text, string from, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            ConcurrentDictionary<string, string> translatedDictionary = new ConcurrentDictionary<string, string>();
            foreach (KeyValuePair<string, string> language in TranslationServices.Core.TranslationServiceFacade.AvailableLanguages)
            {
                string translation = await TranslationServices.Core.TranslationServiceFacade.TranslateStringAsync(text, from, language.Key, category, contentType);
                if (!translatedDictionary.TryAdd(language.Key, translation))
                {
                    Debug.WriteLine("Failed to add: " + language);
                }
            };
            return translatedDictionary;
        }


        public async Task<string> TranslateToAllLanguagesString(string text, string from, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            StringWriter stringWriter = new StringWriter();
            ConcurrentDictionary<string, string> translatedDictionary = new ConcurrentDictionary<string, string>();
            translatedDictionary = await TranslateToAllLanguages(text, from, category, contentType); 
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
