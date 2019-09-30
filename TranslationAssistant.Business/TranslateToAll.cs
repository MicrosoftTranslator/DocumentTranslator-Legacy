using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{
    public class TranslateToAll
    {

        public async Task<SortedDictionary<string, string>> TranslateToAllLanguages(string text, string from, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            SortedDictionary<string, string> translatedDictionary = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> language in TranslationServices.Core.TranslationServiceFacade.AvailableLanguages)
            {
                string translation = await TranslationServices.Core.TranslationServiceFacade.TranslateStringAsync(text, from, language.Key, category, contentType);
                translatedDictionary.Add(language.Key, translation);
            };
            return translatedDictionary;
        }

        public async Task<string> TranslateToAllLanguagesString(string text, string from, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            StringWriter stringWriter = new StringWriter();
            SortedDictionary<string, string> translatedDictionary = new SortedDictionary<string, string>();
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
