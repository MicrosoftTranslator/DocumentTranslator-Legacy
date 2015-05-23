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
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

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
        /// Call once to initialize the static variables
        /// </summary>
        public static void Initialize()
        {
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
            string fromCode = string.Empty;
            string toCode = string.Empty;

            if (from.ToLower() == "Auto-Detect".ToLower() || from == string.Empty)
            {
                fromCode = string.Empty;
            }
            else
            {
                fromCode = AvailableLanguages.First(t => t.Value == from).Key;
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

            // Set Authorization header before sending the request
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty { Method = "POST" };

            httpRequestProperty.Headers.Add("Authorization", headerValue);

            // Creates a block within which an OperationContext object is in scope.
            using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
                    httpRequestProperty;

                if (String.IsNullOrEmpty(toCode))
                {
                    toCode = "en";
                }

                TranslateOptions options = new TranslateOptions();
                options.Category = _CategoryID;

                var translatedTexts = client.TranslateArray(
                    string.Empty,
                    texts,
                    fromCode,
                    toCode,
                    options);
                string[] res = translatedTexts.Select(t => t.TranslatedText).ToArray();
                return res;
            }
        }

        #endregion
    }
}