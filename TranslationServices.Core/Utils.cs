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

    #endregion

    
    public class Utils
    {
        #region Public Methods and Operators

        private static string _ClientID;
        public static string ClientID {
            get {return _ClientID;}
            set { _ClientID = value; }
        }

        private static string _ClientSecret;
        public static string ClientSecret
        {
            get { return _ClientSecret; }
            set { _ClientSecret = value; }
        }


        public static string GetAccesToken()
        {
            // Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            // Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 

            if (string.IsNullOrEmpty(_ClientSecret) || string.IsNullOrEmpty(_ClientID))
            {
                throw new ArgumentException(
                    "Client ID and Client Secret are required. Please obtain your credentials from https://datamarket.azure.com/developer/applications/ and update in Settings.");
            }

            AdmAuthentication admAuth = new AdmAuthentication(_ClientID, _ClientSecret);
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

        #endregion
    }
}