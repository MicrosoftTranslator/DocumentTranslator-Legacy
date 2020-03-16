#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
#endregion

namespace TranslationAssistant.TranslationServices.Core
{
    public static partial class TranslationServiceFacade
    {
        /// <summary>
        /// Raw function to translate an array of strings. Does not allow elements to be larger than <see cref="Maxrequestsize"/>.
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="category"></param>
        /// <param name="contentType"></param>
        /// <param name="retrycount"></param>
        /// <returns></returns>
        private static async Task<string[]> TranslateV3AsyncPolly(
            string[] texts,
            string from,
            string to,
            string category,
            ContentType contentType,
            int retrycount = 3)
        {
            if (UseCustomEndpoint)
            {
                List<string> results = new List<string>();
                foreach (string text in texts)
                {
                    string translationresult = await ContainerTranslateTextAsync(text, from, to).ConfigureAwait(false);
                    results.Add(translationresult);
                }
                return results.ToArray();
            }
            else
            {
                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                    retryCount: 20,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(500)
                    );
                string path = "/translate?api-version=3.0";
                if (ShowExperimental) path += "&flight=experimental";
                string params_ = "&from=" + from + "&to=" + to;
                string thiscategory = category;
                if (String.IsNullOrEmpty(category))
                {
                    thiscategory = null;
                }
                else
                {
                    if (thiscategory == "generalnn") thiscategory = null;
                    if (thiscategory == "general") thiscategory = null;
                }
                if (thiscategory != null) params_ += "&category=" + System.Web.HttpUtility.UrlEncode(category);
                if (contentType == ContentType.HTML) params_ += "&textType=HTML";
                string uri = EndPointAddressV3Public + path + params_;
                if (UseAzureGovernment) uri = EndPointAddressV3Gov + path + params_;


                ArrayList requestAL = new ArrayList();
                foreach (string text in texts)
                {
                    requestAL.Add(new { Text = text });
                }
                string requestJson = JsonConvert.SerializeObject(requestAL);

                IList<string> resultList = new List<string>();
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", AzureKey);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        int status = (int)response.StatusCode;
                        switch (status)
                        {
                            case 429:
                            case 500:
                            case 503:
                                if (texts.Length > 1)
                                {
                                    for (int i = 0; i < texts.Length; i++)
                                    {
                                        try
                                        {
                                            string[] totranslate = new string[1];
                                            totranslate[0] = texts[i];
                                            string[] result = new string[1];
                                            result = await TranslateV3AsyncInternal(totranslate, from, to, category, contentType, 2).ConfigureAwait(false);
                                            resultList.Add(result[0]);
                                        }
                                        catch
                                        {
                                            System.Diagnostics.Debug.WriteLine("Failed to translate: {0}\n", texts[i]);
                                            resultList.Add(texts[i]);
                                        }
                                    }
                                    return resultList.ToArray();
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Retry #" + retrycount + " Response: " + (int)response.StatusCode);
                                    Thread.Sleep(MillisecondsTimeout);
                                    if (retrycount-- <= 0) break;
                                    else await TranslateV3AsyncInternal(texts, from, to, category, contentType, retrycount).ConfigureAwait(false);
                                    break;
                                }
                            default:
                                var errorstring = "ERROR " + response.StatusCode + "\n" + JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);
                                Exception ex = new Exception(errorstring);
                                throw ex;
                        }
                    }
                    JArray jaresult;
                    try
                    {
                        jaresult = JArray.Parse(responseBody);
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine(responseBody);
                        throw;
                    }
                    foreach (JObject result in jaresult)
                    {
                        string txt = (string)result.SelectToken("translations[0].text");
                        resultList.Add(txt);
                    }
                }
                return resultList.ToArray();
            }
        }

    }
}
