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

namespace TranslationAssistant.TranslationServices.Core
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;

    using TranslationAssistant.TranslationServices.Core.TranslatorService;

    #endregion

    public class TranslationServiceFacade
    {
        #region Static Fields
        
        public static Dictionary<string, string> AvailableLanguages = new Dictionary<string, string>();

        private static string _CategoryID;
        public static string CategoryID{
            get { return _CategoryID; }
            set { _CategoryID = value; }
        }

        private static string _ClientID;
        public static string ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }

        private static string _ClientSecret;
        public static string ClientSecret
        {
            get { return _ClientSecret; }
            set { _ClientSecret = value; }
        }


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Check if the Translation service is ready to use, with a valid client ID and secret
        /// </summary>
        /// <returns>true if ready, false if not</returns>
        public static bool IsTranslationServiceReady()
        {
            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;
            try
            {
                string headerValue = "Bearer " + Utils.GetAccesToken();
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Test if a given category value is a valid category in the system
        /// </summary>
        /// <param name="category">Category ID</param>
        /// <returns>True if the category is valid</returns>
        public static bool IsCategoryValid(string category)
        {
            if (category == string.Empty) return true;
            if (category == "") return true;

            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;

            bool returnvalue = true;
            //it may take a while until the category is loaded on server
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    string headerValue = "Bearer " + Utils.GetAccesToken();
                    var bind = new BasicHttpBinding { Name = "BasicHttpBinding_LanguageService" };
                    var epa = new EndpointAddress("http://api.microsofttranslator.com/V2/soap.svc");
                    LanguageServiceClient client = new LanguageServiceClient(bind, epa);
                    client.Translate(headerValue, "Test", "en", "fr", "text/plain", category);
                    returnvalue = true;
                    break;
                }
                catch {
                    returnvalue = false;
                    Thread.Sleep(1000);
                    continue;
                }
            }
            return returnvalue;
        }



        /// <summary>
        /// Call once to initialize the static variables
        /// </summary>
        public static void Initialize()
        {
            LoadCredentials();
            if (!IsTranslationServiceReady()) return;
            var bind = new BasicHttpBinding { Name = "BasicHttpBinding_LanguageService" };
            var epa = new EndpointAddress("http://api.microsofttranslator.com/V2/soap.svc");
            LanguageServiceClient client = new LanguageServiceClient(bind, epa);
            string headerValue = "Bearer " + Utils.GetAccesToken();

            string[] languages = client.GetLanguagesForTranslate(headerValue);
            string[] languagenames = client.GetLanguageNames(headerValue, "en", languages, false);
            for (int i = 0; i < languages.Length; i++)
            {
                if (!AvailableLanguages.ContainsKey(languages[i]))
                {
                    AvailableLanguages.Add(languages[i], languagenames[i]);
                }
            }
            client.Close();
        }

        /// <summary>
        /// Loads credentials from settings file.
        /// Doesn't need to be public, because it is called during Initialize();
        /// </summary>
        private static void LoadCredentials()
        {
            _ClientID = Properties.Settings.Default.ClientID;
            _ClientSecret = Properties.Settings.Default.ClientSecret;
            _CategoryID = Properties.Settings.Default.CategoryID;
        }

        /// <summary>
        /// Saves credentials ClientID, clientSecret and categoryID to the personalized settings file.
        /// </summary>
        public static void SaveCredentials()
        {
            Properties.Settings.Default.ClientID = _ClientID;
            Properties.Settings.Default.ClientSecret = _ClientSecret;
            Properties.Settings.Default.CategoryID = _CategoryID;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Takes a single language name and returns the matching language code. OK to pass a language code.
        /// </summary>
        /// <param name="languagename"></param>
        /// <returns></returns>
        public static string LanguageNameToLanguageCode(string languagename)
        {
            if (AvailableLanguages.ContainsKey(languagename))
            {
                return languagename;
            }
            else if (AvailableLanguages.ContainsValue(languagename))
            {
                return AvailableLanguages.First(t => t.Value == languagename).Key;
            }
            else
            {
                throw new ArgumentException(String.Format("LanguageNameToLanguageCode: Language name {0} not found.", languagename));
            }
        }

        public static string LanguageCodeToLanguageName(string languagecode)
        {
            if (AvailableLanguages.ContainsValue(languagecode))
            {
                return languagecode;
            }
            else if (AvailableLanguages.ContainsKey(languagecode))
            {
                return AvailableLanguages.First(t => t.Key == languagecode).Value;
            }
            else
            {
                throw new ArgumentException(String.Format("LanguageCodeToLanguageName: Language code {0} not found.", languagecode));
            }
        }

        /// <summary>
        /// Translates an array of strings from the from langauge code to the to language code.
        /// From langauge code can stay empty, in that case the source language is auto-detected, across all elements of the array together.
        /// </summary>
        /// <param name="texts">Array of strings to translate</param>
        /// <param name="from">From language code. May be empty</param>
        /// <param name="to">To language code. Must be a valid language</param>
        /// <returns></returns>
        public static string[] TranslateArray(string[] texts, string from, string to)
        {
            return TranslateArray(texts, from, to, "text/plain");
        }
        /// <summary>
        /// Translates a string
        /// </summary>
        /// <param name="text">String to translate</param>
        /// <param name="from">From language</param>
        /// <param name="to">To language</param>
        /// <param name="contentType">Content Type</param>
        /// <returns></returns>
        public static string TranslateString(string text, string from, string to, string contentType)
        {
            string[] texts = new string[1];
            texts[0] = text;
            string[] results = TranslateArray(texts, from, to, contentType);
            return results[0];
        }


        /// <summary>
        /// Translates an array of strings from the from langauge code to the to language code.
        /// From langauge code can stay empty, in that case the source language is auto-detected, across all elements of the array together.
        /// </summary>
        /// <param name="texts">Array of strings to translate</param>
        /// <param name="from">From language code. May be empty</param>
        /// <param name="to">To language code. Must be a valid language</param>
        /// <param name="contentType">Whether this is plain text or HTML</param>
        /// <returns></returns>
        public static string[] TranslateArray(string[] texts, string from, string to, string contentType)
        {
            string fromCode = string.Empty;
            string toCode = string.Empty;

            if (from.ToLower() == "Auto-Detect".ToLower() || from == string.Empty)
            {
                fromCode = string.Empty;
            }
            else
            {
                try { fromCode = AvailableLanguages.First(t => t.Value == from).Key; }
                catch { fromCode = from; }
            }

            toCode = LanguageNameToLanguageCode(to);

            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;
            string headerValue = "Bearer " + Utils.GetAccesToken();
            var bind = new BasicHttpBinding
                           {
                               Name = "BasicHttpBinding_LanguageService",
                               OpenTimeout = TimeSpan.FromMinutes(5),
                               CloseTimeout = TimeSpan.FromMinutes(5),
                               ReceiveTimeout = TimeSpan.FromMinutes(5),
                               MaxReceivedMessageSize = int.MaxValue,
                               MaxBufferPoolSize = int.MaxValue,
                               MaxBufferSize = int.MaxValue,
                               Security =
                                   new BasicHttpSecurity { Mode = BasicHttpSecurityMode.Transport }
                           };

            var epa = new EndpointAddress("https://api.microsofttranslator.com/V2/soap.svc");
            LanguageServiceClient client = new LanguageServiceClient(bind, epa);

            if (String.IsNullOrEmpty(toCode))
            {
                toCode = "en";
            }

            TranslateOptions options = new TranslateOptions();
            options.Category = _CategoryID;
            options.ContentType = contentType;

            try
            {
                var translatedTexts = client.TranslateArray(
                    headerValue,
                    texts,
                    fromCode,
                    toCode,
                    options);
                string[] res = translatedTexts.Select(t => t.TranslatedText).ToArray();
                return res;
            }
            catch   //try again forcing English as source language
            {
                var translatedTexts = client.TranslateArray(
                    headerValue,
                    texts,
                    "en",
                    toCode,
                    options);
                string[] res = translatedTexts.Select(t => t.TranslatedText).ToArray();
                return res;
            }
        }

        /// <summary>
        /// Breaks a piece of text into sentences and returns an array containing the lengths in each sentence. 
        /// </summary>
        /// <param name="text">The text to analyze and break.</param>
        /// <param name="languageID">The language identifier to use.</param>
        /// <returns>An array of integers representing the lengths of the sentences. The length of the array is the number of sentences, and the values are the length of each sentence.</returns>
        public static int[] BreakSentences(string text, string languageID)
        {
            var bind = new BasicHttpBinding
                           {
                               Name = "BasicHttpBinding_LanguageService",
                               OpenTimeout = TimeSpan.FromMinutes(5),
                               CloseTimeout = TimeSpan.FromMinutes(5),
                               ReceiveTimeout = TimeSpan.FromMinutes(5),
                               MaxReceivedMessageSize = int.MaxValue,
                               MaxBufferPoolSize = int.MaxValue,
                               MaxBufferSize = int.MaxValue,
                               Security =
                                   new BasicHttpSecurity { Mode = BasicHttpSecurityMode.Transport }
                           };

            var epa = new EndpointAddress("https://api.microsofttranslator.com/V2/soap.svc");
            TranslatorService.LanguageServiceClient client = new LanguageServiceClient(bind, epa);
            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;
            string headerValue = "Bearer " + Utils.GetAccesToken();
            return client.BreakSentences(headerValue, text, languageID);
        }

        /// <summary>
        /// Breaks a piece of text into sentences and returns an array containing the lengths in each sentence. 
        /// </summary>
        /// <param name="text">The text to analyze and break.</param>
        /// <param name="languageID">The language identifier to use.</param>
        /// <returns>An array of integers representing the lengths of the sentences. The length of the array is the number of sentences, and the values are the length of each sentence.</returns>
        async public static Task<int[]> BreakSentencesAsync(string text, string languageID)
        {
            var bind = new BasicHttpBinding
            {
                Name = "BasicHttpBinding_LanguageService",
                OpenTimeout = TimeSpan.FromMinutes(5),
                CloseTimeout = TimeSpan.FromMinutes(5),
                ReceiveTimeout = TimeSpan.FromMinutes(5),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                Security =
                    new BasicHttpSecurity { Mode = BasicHttpSecurityMode.Transport }
            };

            var epa = new EndpointAddress("https://api.microsofttranslator.com/V2/soap.svc");
            TranslatorService.LanguageServiceClient client = new LanguageServiceClient(bind, epa);
            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;
            string headerValue = "Bearer " + Utils.GetAccesToken();
            Task<int[]> BSTask = client.BreakSentencesAsync(headerValue, text, languageID);
            int[] result = await BSTask;
            return result;
        }



        /// <summary>
        /// Adds a translation to Microsoft Translator's translation memory.
        /// </summary>
        /// <param name="originalText">Required. A string containing the text to translate from. The string has a maximum length of 1000 characters.</param>
        /// <param name="translatedText">Required. A string containing translated text in the target language. The string has a maximum length of 2000 characters. </param>
        /// <param name="from">Required. A string containing the language code of the source language. Must be a valid culture name. </param>
        /// <param name="to">Required. A string containing the language code of the target language. Must be a valid culture name. </param>
        /// <param name="rating">Optional. An int representing the quality rating for this string. Value between -10 and 10. Defaults to 1. </param>
        /// <param name="user">Required. A string used to track the originator of the submission. </param>
        public static void AddTranslation(string originalText, string translatedText, string from, string to, int rating, string user)
        {
            var bind = new BasicHttpBinding
            {
                Name = "BasicHttpBinding_LanguageService",
                OpenTimeout = TimeSpan.FromMinutes(5),
                CloseTimeout = TimeSpan.FromMinutes(5),
                ReceiveTimeout = TimeSpan.FromMinutes(5),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                Security =
                    new BasicHttpSecurity { Mode = BasicHttpSecurityMode.Transport }
            };

            var epa = new EndpointAddress("https://api.microsofttranslator.com/V2/soap.svc");
            TranslatorService.LanguageServiceClient client = new LanguageServiceClient(bind, epa);
            Utils.ClientID = _ClientID;
            Utils.ClientSecret = _ClientSecret;
            string headerValue = "Bearer " + Utils.GetAccesToken();
            client.AddTranslation(headerValue, originalText, translatedText, from, to, rating, "text/plain", _CategoryID, user, string.Empty);
            return;
        }

        #endregion
    }
}