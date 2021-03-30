// // ----------------------------------------------------------------------
// // <copyright file="TranslationServiceFacade.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>TranslationServiceFacade.cs</summary>
// // ----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    public static class AvailableLanguages
    {
        private static Dictionary<string, string> languages = new Dictionary<string, string>();

        public static bool UseCustomEndpoint { get; set; } = false;
        public static bool ShowExperimental { get; set; } = false;
        private static bool LastShowExperimental = false;
        private static string LastAcceptLanguage = string.Empty;
        public static string EndPointAddress = "https://api.cognitive.microsofttranslator.com";

        public static Dictionary<string, string> GetLanguages()
        {
            _ = GetLanguagesWrapper(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            lock (languages)
            {
                return languages;
            }
        }

        static AvailableLanguages()
        {
            _ = GetLanguagesWrapper(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }


        /// <summary>
        /// Fills the AvailableLanguages dictionary
        /// </summary>
        /// <param name="AcceptLanguage">Accept-Language</param>
        public static async Task<Dictionary<string, string>> GetLanguages(string AcceptLanguage)
        {
            if (string.IsNullOrEmpty(AcceptLanguage)) AcceptLanguage = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if ((AcceptLanguage == LastAcceptLanguage) && (ShowExperimental == LastShowExperimental) && (languages.Count > 3))
            {
                return languages;
            }
            else return await GetLanguagesWrapper(AcceptLanguage);
        }

        private static async Task<Dictionary<string, string>> GetLanguagesWrapper(string AcceptLanguage)
        {
            int retrycounter = 2;
            while (retrycounter >= 0)
            {
                retrycounter--;
                try
                {
                    await GetLanguagesInternal(AcceptLanguage);
                    return languages;
                }
                catch
                {
                    if (retrycounter <= 0) throw;
                    await Task.Delay(1000); //wait one second
                }
            }
            return null;
        }



        /// <summary>
        /// Fills the AvailableLanguages dictionary
        /// </summary>
        /// <param name="AcceptLanguage">Accept-Language</param>
        private static async Task GetLanguagesInternal(string AcceptLanguage = "en")
        {
            lock (languages)
            {
                languages.Clear();
            }
            if (UseCustomEndpoint)
            {
                languages = await TranslationServiceFacade.ContainerGetLanguages();
                return;
            }
            string uri = EndPointAddress + "/languages?api-version=3.0&scope=translation";
            if (ShowExperimental) uri += "&flight=experimental";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpRequestMessage request = new HttpRequestMessage();
                client.Timeout = TimeSpan.FromSeconds(10);
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Accept-Language", AcceptLanguage);
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                lock (languages)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(jsonResponse);
                        var langs = result["translation"];

                        string[] languagecodes = languages.Keys.ToArray();
                        foreach (var kv in langs)
                        {
                            languages.Add(kv.Key, kv.Value["name"]);
                        }
                    }
                }
                Debug.Assert(languages.Count > 0, "GetLanguagesInternal: Zero languages.");
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("ERROR: Request exception while retrieving languages.");
                lock (languages)
                {
                    languages.Clear();
                }
                return;
            }
            return;
        }

        internal static void Clear()
        {
            lock (languages)
            {
                languages.Clear();
            }
        }
    }
}