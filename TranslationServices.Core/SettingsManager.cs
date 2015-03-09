// // ----------------------------------------------------------------------
// // <copyright file="SettingsManager.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>SettingsManager.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.TranslationServices.Core
{
    #region

    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    #endregion

    public static class SettingsManager
    {
        #region Public Methods and Operators

        public static string GetClientId()
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

            if (config == null)
            {
                throw new ArgumentException("Not able to locate or load the config file name FileTranslator.config.");
            }

            var section = (AppSettingsSection)config.GetSection("appSettings");
            return section.Settings["ClientID"].Value;
        }

        public static string GetClientSecret()
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

            if (config == null)
            {
                throw new ArgumentException("Not able to locate or load the config file name FileTranslator.config.");
            }

            var section = (AppSettingsSection)config.GetSection("appSettings");
            return section.Settings["ClientSecret"].Value;
        }

        public static string GetCategoryID()
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

            if (config == null)
            {
                throw new ArgumentException("Not able to locate or load the config file name FileTranslator.config.");
            }

            var section = (AppSettingsSection)config.GetSection("appSettings");
            return section.Settings["CategoryID"].Value;
        }

        #endregion
    }
}