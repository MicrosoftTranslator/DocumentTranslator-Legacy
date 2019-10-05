using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TranslationAssistant.Business
{
    public class TranslateToAll
    {
        /// <summary>
        /// Translate a string to all enumerated languages
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="from">FROM language code</param>
        /// <param name="category">Category ID</param>
        /// <param name="contentType">Content Type plain text or HTML</param>
        /// <returns></returns>
        public async Task<SortedDictionary<string, string>> TranslateToAllLanguages(string text, string from, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            SortedDictionary<string, string> translatedDictionary = new SortedDictionary<string, string>();
            List<Task<Translation>> tasks = new List<Task<Translation>>();
            foreach (KeyValuePair<string, string> language in TranslationServices.Core.TranslationServiceFacade.AvailableLanguages)
            {
                Task<Translation> task = translateDictionaryEntry(text, from, language.Key, category, contentType);
                tasks.Add(task);
            };
            await Task.WhenAll(tasks);
            foreach (Task<Translation> task in tasks)
            {
                Translation translation = await task;
                translatedDictionary.Add(translation.language, translation.text);
            }
            return translatedDictionary;
        }

        private class Translation
        {
            public string language;
            public string text;
            public Translation(string language, string text)
            {
                this.language = language;
                this.text = text;
            }
        };

        private async Task<Translation> translateDictionaryEntry(string text, string from, string to, string category, TranslationServices.Core.TranslationServiceFacade.ContentType contentType)
        {
            string result = await TranslationServices.Core.TranslationServiceFacade.TranslateStringAsync(text, from, to, category, contentType);
            Translation translation = new Translation(to, result);
            return translation;
        }

        /// <summary>
        /// Build a string containing the language code followed by 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="category"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
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
