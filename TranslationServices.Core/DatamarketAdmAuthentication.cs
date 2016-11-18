// // ----------------------------------------------------------------------
// // <copyright file="DatamarketAdmAuthentication.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>DatamarketAdmAuthentication.cs</summary>
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

    public class DatamarketAdmAuthentication : AdmAuthentication
    {
        #region Fields

        private readonly string request;

        #endregion

        #region Constructors and Destructors

        public DatamarketAdmAuthentication(string clientId, string clientSecret)
        {
            this.request =
                string.Format(
                    "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
                    HttpUtility.UrlEncode(clientId),
                    HttpUtility.UrlEncode(clientSecret));
        }

        #endregion

        #region Public Methods and Operators

        public override AdmAccessToken GetAccessToken()
        {
            return this.HttpPost(Constants.DatamarketAccessUri, this.request);
        }

        #endregion

        #region Methods

        private AdmAccessToken HttpPost(string datamarketAccessUri, string requestDetails)
        {
            WebRequest webRequest = WebRequest.Create(datamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));

                // Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }

        #endregion
    }
}