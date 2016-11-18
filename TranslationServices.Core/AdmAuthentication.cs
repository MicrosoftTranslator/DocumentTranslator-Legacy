// // ----------------------------------------------------------------------
// // <copyright file="AdmAuthentication.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AdmAuthentication.cs</summary>
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

    public abstract class AdmAuthentication
    {
        #region Public Methods and Operators

        public abstract AdmAccessToken GetAccessToken();

        #endregion
    }
}