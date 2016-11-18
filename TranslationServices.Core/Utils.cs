// // ----------------------------------------------------------------------
// // <copyright file="Utils.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>Utils.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.TranslationServices.Core
{
    #region Usings

    using System;
    using System.IO;
    using System.Net;

    #endregion Usings

    public class Utils
    {
        #region Public Methods and Operators

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

        private static string _SubscriptionKey;

        public static string SubscriptionKey
        {
            get { return _SubscriptionKey; }
            set { _SubscriptionKey = value; }
        }

        public static string GetAccesToken()
        {
            AdmAuthentication admAuth;
            if (string.IsNullOrEmpty(_SubscriptionKey))
            {
                // Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
                // Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)

                if (string.IsNullOrEmpty(_ClientSecret) || string.IsNullOrEmpty(_ClientID))
                {
                    throw new ArgumentException(
                        "Client ID and Client Secret are required. Please obtain your credentials from https://datamarket.azure.com/developer/applications/ and update in Settings.");
                }

                admAuth = new DatamarketAdmAuthentication(_ClientID, _ClientSecret);
            }
            else
            {
                // Get Subscription keys from https://portal.azure.com/ => All resources => (Your Cognitive Services account) => Resource Management => Keys
                // Refer obtaining AccessToken (http://docs.microsofttranslator.com/oauth-token.html)

                admAuth = new AzureAdmAuthentication(_SubscriptionKey);
            }
            try
            {
                AdmAccessToken admToken = admAuth.GetAccessToken();
                return admToken.access_token;
            }
            catch (WebException e)
            {
                // Obtain detailed error information
                string strResponse = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)e.Response)
                {
                    if (response != null)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                            {
                                strResponse = sr.ReadToEnd();
                            }
                        }
                    }
                }

                throw new Exception(string.Format("Http status code={0}, error message={1}", e.Status, strResponse), e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Public Methods and Operators
    }
}