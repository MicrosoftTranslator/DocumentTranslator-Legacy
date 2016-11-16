// // ----------------------------------------------------------------------
// // <copyright file="AzureAdmAuthentication.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AzureAdmAuthentication.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.TranslationServices.Core
{
    #region

    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web;

    #endregion

    public class AzureAdmAuthentication : AdmAuthentication
    {
        #region Fields

        private readonly string subscriptionKey;

        #endregion

        #region Constructors and Destructors

        public AzureAdmAuthentication(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
        }

        #endregion

        #region Public Methods and Operators

        public override AdmAccessToken GetAccessToken()
        {
            return this.HttpPost(Constants.AzureAccessUri, this.subscriptionKey);
        }

        #endregion

        #region Methods

        private AdmAccessToken HttpPost(string AzureAccessUri, string subscriptionKey)
        {
            var webRequest = HttpWebRequest.Create(AzureAccessUri);
            webRequest.ContentType = "application/json";
            ((HttpWebRequest)webRequest).Accept = "application/jwt";
            webRequest.Headers.Set("Ocp-Apim-Subscription-Key", subscriptionKey);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(new byte[0], 0, 0);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                var tokenString = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();

                AdmAccessToken token = new AdmAccessToken();
                token.access_token = tokenString;
                return token;
            }
        }

        #endregion
    }
}