using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TranslationAssistant.TranslationServices.Core;

namespace TranslationAssistant.Business
{
    public class TranslateToAll
    {
        public event EventHandler OneTranslationDone;

 
        /// <summary>
        /// Translate to all languages
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="category"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private async Task<SortedDictionary<string, string>> TranslateToAllLanguages(string text, string from, string category, TranslationServiceFacade.ContentType contentType)
        {
            EventHandler handler = OneTranslationDone;
            List<Task<KeyValuePair<string, string>>> tasklist = new List<Task<KeyValuePair<string, string>>>();
            foreach (KeyValuePair<string, string> language in TranslationServiceFacade.AvailableLanguages)
            {
                Task<KeyValuePair<string, string>> task = TranslateInternal(text, from, language.Key, category, contentType);
                tasklist.Add(task);
                handler(this, EventArgs.Empty);
            };
            KeyValuePair<string, string>[] resultkv = await Task.WhenAll(tasklist); 
            SortedDictionary<string, string> translatedDictionary = new SortedDictionary<string, string>();
            foreach(KeyValuePair<string, string> pair in resultkv)
            {
                translatedDictionary.Add(pair.Key, pair.Value);
            }



            return translatedDictionary;
        }

        /// <summary>
        /// A wrapper for the Translate method that returns a key value pair with [targetlanguage, translatedstring]
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="category"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private async Task<KeyValuePair<string, string>> TranslateInternal(string text, string from, string to, string category, TranslationServiceFacade.ContentType contentType)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>(
                key: to,
                value: await TranslationServiceFacade.TranslateStringAsync(text, from, to, contentType).ConfigureAwait(false)
                );
            return kv;
        }

        /// <summary>
        /// Return a string that contains a sorted sequence of translations to all available languages
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="category"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
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
