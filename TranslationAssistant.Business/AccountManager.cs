// // ----------------------------------------------------------------------
// // <copyright file="AccountManager.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AccountManager.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business
{
    #region Usings

    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using TranslationAssistant.Business.Model;
    using TranslationAssistant.TranslationServices.Core;

    #endregion

    public class AccountManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns account settings
        /// </summary>
        /// <returns></returns>
        public AccountModel GetAccountSettings()
        {
            AccountModel model = new AccountModel
                                     {
                                         ClientID = SettingsManager.GetClientId(),
                                         ClientSecret = SettingsManager.GetClientSecret(),
                                         CategoryID = SettingsManager.GetCategoryID()
                                     };

            return model;
        }

        public void SaveAccountSettings(AccountModel model)
        {
            var configFile = new ExeConfigurationFileMap
                                 {
                                     ExeConfigFilename =
                                         Path.Combine(
                                             Assembly.GetExecutingAssembly()
                                         .Location.Substring(
                                             0,
                                             Assembly.GetExecutingAssembly()
                                         .Location.LastIndexOf(
                                             @"\",
                                             System.StringComparison.Ordinal)),
                                             "FileTranslator.config")
                                 };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(
                configFile,
                ConfigurationUserLevel.None);

            var section = (AppSettingsSection)config.GetSection("appSettings");
            section.Settings["ClientSecret"].Value = model.ClientSecret;
            section.Settings["ClientID"].Value = model.ClientID;
            section.Settings["CategoryID"].Value = model.CategoryID;

            config.Save();
        }

        #endregion
    }
}