using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    public static partial class TranslationServiceFacade
    {
        private static async Task<bool> ContainerStatus()
        {
            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(CustomEndpointUrl);
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        private static async void ContainerGetLanguages()
        {
            AvailableLanguages.Clear();
            AvailableLanguages.Add("en", "English");
            AvailableLanguages.Add("ar", "Arabic");
            AvailableLanguages.Add("de", "German");
            AvailableLanguages.Add("ru", "Russian");
            AvailableLanguages.Add("zh-Hans", "Chinese (Simplified)");
            AvailableLanguages.Add("es", "Spanish");
            AvailableLanguages.Add("fr", "French");

            ///This container probably only contains a subset of these.
            ///Test all of them and delete the ones that aren't valid.
            List<Task<KeyValuePair<string, bool>>> tasks = new List<Task<KeyValuePair<string, bool>>>();
            foreach (KeyValuePair<string, string> kv in AvailableLanguages)
            {
                Task<KeyValuePair<string, bool>> task = ContainerTestLanguage(kv.Key);
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            foreach (var task in tasks)
            {
                if (!task.Result.Value) AvailableLanguages.Remove(task.Result.Key);
            }
        }

        private static async Task<KeyValuePair<string, bool>> ContainerTestLanguage(string language)
        {
            bool testresult = false;
            try
            {
                string translationresult = await ContainerTranslateTextAsync("Test", "en", language).ConfigureAwait(false);
                if (translationresult != null) testresult = true;
            }
            catch { };
            KeyValuePair<string, bool> returnvalue = new KeyValuePair<string, bool>(language, testresult);
            return returnvalue;
        }

        private static string[] ContainerBreakSentences(string input, string language)
        {
            string[] separators = { ". ", "\r\n", "۔", "։", "⽌", "⾉", "。", "︒", "﹒", "．", "｡" };
            return input.Split(separators, StringSplitOptions.None);
        }

        private static async Task<string> ContainerTranslateTextAsync(string textToTranslate, string fromlanguage, string tolanguage)
        {
            string[] arraytotranslate = ContainerBreakSentences(textToTranslate, fromlanguage);
            StringBuilder result = new StringBuilder();
            foreach (string element in arraytotranslate)
            {
                string translationresult = await ContainerTranslateTextAsyncInternal(element, fromlanguage, tolanguage).ConfigureAwait(false);
                result.Append(translationresult + ". ");
            }
            //result.Replace("..", ".");
            return result.ToString();
        }


        private static async Task<string> ContainerTranslateTextAsyncInternal(string textToTranslate, string fromlanguage, string tolanguage)
        {
            if (fromlanguage == tolanguage) return textToTranslate;
            if ((fromlanguage != "en") && (tolanguage != "en"))
            {
                string intermediateresult = await ContainerTranslateTextAsync(textToTranslate, fromlanguage, "en").ConfigureAwait(false);
                string translateresult = await ContainerTranslateTextAsync(intermediateresult, "en", tolanguage).ConfigureAwait(false);
                return translateresult;
            }
            else
            {
                string TranslateApi = "/translate?api-version=3.0&from=" + fromlanguage + "&to=" + tolanguage;
                var body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);
                using (HttpRequestMessage request =
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{CustomEndpointUrl}{TranslateApi}"),
                        Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                    })
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.Timeout = TimeSpan.FromSeconds(30);
                            // Send the request and await a response.
                            var response = await client.SendAsync(request).ConfigureAwait(false);
                            if (response.IsSuccessStatusCode)
                            {
                                string resultJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                                return ParseJsonResult(resultJson)[0];
                            }
                            else return null;
                        }
                    }
                    catch
                    {
                        AvailableLanguages.Clear();
                        return null;
                    }
                }
            }
        }


    }
}
