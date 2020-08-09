// // ----------------------------------------------------------------------
// // <copyright file="SetCredentials.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>SetCredentials.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.TranslationPlugins
{
    using System;

    using TranslationAssistant.AutomationToolkit.BasePlugin;
    using TranslationAssistant.TranslationServices.Core;

    /// <summary>
    ///     To set the credentials for the translation service.
    /// </summary>
    internal class SetCredentials : BasePlugIn
    {
        #region Fields

        /// <summary>
        ///     The source document.
        /// </summary>
        private readonly Argument AzureKey;

        /// <summary>
        ///     The target language.
        /// </summary>
        private readonly Argument categoryID;

        /// <summary>
        ///     The Cloud to use.
        /// </summary>
        private readonly Argument Cloud;

        /// <summary>
        ///     The Region to use. The key in the SetCredentials function must match the region. 
        /// </summary>
        private readonly Argument Region;

        /// <summary>
        /// Reset the credentials to default values.
        /// </summary>
        private readonly Argument Reset;

        /// <summary>
        /// Prints the saved credentials
        /// </summary>
        private readonly Argument Print;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetCredentials" /> class.
        /// </summary>
        /// <param name="Logger">
        ///     The logger.
        /// </param>
        public SetCredentials(ConsoleLogger Logger)
            : base(Logger)
        {
            this.AzureKey = new Argument(
                "APIkey",
                false,
                "API key to use for the calls to the Translator service.") ;

            this.categoryID = new Argument(
                "categoryID",
                false,
                "Custom Translator category ID to use for calls to the translator service.");

            this.Cloud = new Argument(
                "Cloud",
                false,
                new string[] { TranslationServiceFacade.AzureCloud },
                Endpoints.GetClouds(),
                true,
                "The cloud you want to use for Translator calls.");

            this.Region = new Argument(
                "Region",
                false,
                new string[] { TranslationServiceFacade.AzureRegion },
                Endpoints.AvailableRegions.ToArray(),
                true,
                "The region of the resource the APIKey is associated with.");

            this.Reset = new Argument(
                "Reset",
                false,
                "Value of 'true' resets the credentials to their default values.");

            this.Arguments = new ArgumentList(
                new[] { this.AzureKey, this.categoryID, this.Cloud, this.Region, this.Reset},
                Logger);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Sets or resets the credentials for use with the Translator service.";
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SetCredentials";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.Reset.ValueString)) TranslationServiceFacade.ResetCredentials();
                if (!string.IsNullOrEmpty(this.AzureKey.ValueString)) TranslationServiceFacade.AzureKey = this.AzureKey.ValueString;
                if (!string.IsNullOrEmpty(this.categoryID.ValueString)) TranslationServiceFacade.CategoryID = this.categoryID.ValueString;
                if (!string.IsNullOrEmpty(this.Cloud.ValueString)) TranslationServiceFacade.AzureCloud = this.Cloud.ValueString;
                if (!string.IsNullOrEmpty(this.Region.ValueString)) TranslationServiceFacade.AzureRegion = this.Region.ValueString;
                TranslationServiceFacade.SaveCredentials();
            }
            catch (Exception ex)
            {
                this.Logger.WriteException(ex);
                Console.ReadLine();
                return false;
            }

            this.Logger.WriteLine(LogLevel.Msg, string.Format("Credentials saved."));

            if (TranslationServiceFacade.IsTranslationServiceReady())
            {
                this.Logger.WriteLine(LogLevel.Msg, string.Format("Translator service is ready to use."));
            }
            else
            {
                this.Logger.WriteLine(LogLevel.Error, string.Format("API Key is invalid. Check that the key is for a resource in this cloud, in this region."));
            }
            return true;
        }

        #endregion
    }
}