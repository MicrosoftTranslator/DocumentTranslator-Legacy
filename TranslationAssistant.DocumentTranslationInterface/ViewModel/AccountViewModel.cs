// // ----------------------------------------------------------------------
// // <copyright file="AccountViewModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AccountViewModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.ViewModel
{
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;
    using TranslationAssistant.DocumentTranslationInterface.Common;

    /// <summary>
    ///     The account view model.
    /// </summary>
    public class AccountViewModel : Notifyer
    {
        #region Fields

        /// <summary>
        ///     The application id.
        /// </summary>
        private string _AzureKey;

        
        /// <summary>
        ///     The category identifier.
        /// </summary>
        private string categoryID;

        /// <summary>
        /// The cloud to use 
        /// </summary>
        private string azureCloud;

        /// <summary>
        /// The region to use 
        /// </summary>
        private string azureRegion;

        /// <summary>
        /// Show the experimental languages 
        /// </summary>
        private bool showExperimental;

        /// <summary>
        /// Use the Container 
        /// </summary>
        private bool useCustomEndpoint;

        /// <summary>
        /// Container Url 
        /// </summary>
        private string customEndpointUrl;
        
        /// <summary>
        ///     The save account settings click command.
        /// </summary>
        private ICommand saveAccountSettingsClickCommand;

        /// <summary>
        ///     The status text.
        /// </summary>
        private string statusText;

        #endregion
                
        #region Public Properties

        /// <summary>
        ///     Gets or sets the application id.
        /// </summary>
        public string AzureKey
        {
            get
            {
                return this._AzureKey;
            }

            set
            {
                this._AzureKey= value;
                this.NotifyPropertyChanged("AzureKey");
            }
        }

        
        public string CategoryID
        {
            get
            {
                return this.categoryID;
            }

            set
            {
                this.categoryID = value;
                this.NotifyPropertyChanged("CategoryID");
            }
        }

        public string[] AvailableClouds
        {
            get
            {
                return TranslationAssistant.TranslationServices.Core.Endpoints.GetClouds();
            }

            set
            {
                this.AvailableClouds = value;
                this.NotifyPropertyChanged("AvailableClouds");
            }
        }
        
        public string SelectedRegion
        {
            get
            {
                return TranslationAssistant.TranslationServices.Core.TranslationServiceFacade.AzureRegion;
            }

            set
            {
                this.SelectedRegion = value;
                this.NotifyPropertyChanged("SelectedRegion");
            }

        }

        public string[] AvailableRegions
        {
            get
            {
                return TranslationAssistant.TranslationServices.Core.Endpoints.AvailableRegions.ToArray();
            }

            set
            {
                this.AvailableRegions = value;
                this.NotifyPropertyChanged("AvailableRegions");
            }
        }
        public bool ShowExperimental
        {
            get
            {
                return this.showExperimental;
            }

            set
            {
                this.showExperimental = value;
                this.NotifyPropertyChanged("ShowExperimental");
            }
        }

        public string AzureRegion
        {
            get
            {
                return this.azureRegion;
            }
            set
            {
                this.azureRegion = value;
                this.NotifyPropertyChanged("AzureRegion");
            }
        }

        public string AzureCloud
        {
            get
            {
                return this.azureCloud;
            }
            set
            {
                this.azureCloud = value;
                this.NotifyPropertyChanged("AzureCloud");
            }
        }



        public bool UseCustomEndpoint
        {
            get
            {
                return this.useCustomEndpoint;
            }

            set
            {
                this.useCustomEndpoint = value;
                this.NotifyPropertyChanged("UseCustomEndpoint");
            }
        }

        public string CustomEndpointUrl
        {
            get
            {
                return this.customEndpointUrl.ToString();
            }

            set
            {
                customEndpointUrl = value;
                NotifyPropertyChanged("CustomEndpointUrl");
            }
        }

        /// <summary>
        ///     Gets the save account settings click command.
        /// </summary>
        public ICommand SaveAccountSettingsClickCommand
        {
            get
            {
                return this.saveAccountSettingsClickCommand
                       ?? (this.saveAccountSettingsClickCommand = new DelegateCommand(this.SaveAccountClick));
            }
        }

        /// <summary>
        ///     Gets or sets the status text.
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                this.statusText = value;
                this.NotifyPropertyChanged("StatusText");
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load the account settings from the DocumentTranslator.settings file, which is actually in the user appsettings folder and named user.config.
        /// </summary>
        public AccountViewModel()
        {
            //Initialize in order to load the credentials.
            _ = TranslationServices.Core.TranslationServiceFacade.Initialize();
            this.AzureKey = TranslationServices.Core.TranslationServiceFacade.AzureKey;
            this.categoryID = TranslationServices.Core.TranslationServiceFacade.CategoryID;
            this.AzureRegion = TranslationServices.Core.TranslationServiceFacade.AzureRegion;
            this.AzureCloud = TranslationServices.Core.TranslationServiceFacade.AzureCloud;
            this.useCustomEndpoint = TranslationServices.Core.TranslationServiceFacade.UseCustomEndpoint;
            this.customEndpointUrl = TranslationServices.Core.TranslationServiceFacade.CustomEndpointUrl;
            this.showExperimental = TranslationServices.Core.TranslationServiceFacade.ShowExperimental;
        }

        /// <summary>
        ///     Saves the account settings to the settings file for next use.
        /// </summary>
        private async void SaveAccountClick()
        {
            //Set the Account values and save.
            TranslationServices.Core.TranslationServiceFacade.AzureKey = TranslationServices.Core.TranslationServiceFacade.AzureKey.Trim();
            TranslationServices.Core.TranslationServiceFacade.CategoryID = this.categoryID.Trim();
            TranslationServices.Core.TranslationServiceFacade.AzureCloud = this.azureCloud;
            TranslationServices.Core.TranslationServiceFacade.AzureRegion = this.azureRegion;
            TranslationServices.Core.TranslationServiceFacade.UseCustomEndpoint = this.useCustomEndpoint;
            TranslationServices.Core.TranslationServiceFacade.CustomEndpointUrl = this.customEndpointUrl;
            TranslationServices.Core.TranslationServiceFacade.ShowExperimental = this.showExperimental;
            TranslationServices.Core.TranslationServiceFacade.SaveCredentials();
            _ = TranslationServices.Core.TranslationServiceFacade.Initialize(true);

            bool isready = false;

            try { isready = await TranslationServices.Core.TranslationServiceFacade.IsTranslationServiceReadyAsync(); }
            catch { isready = false; }

            if (isready) {
                this.StatusText = Properties.Resources.Common_SettingsSaved;
                NotifyPropertyChanged("SettingsSaved");
                //Need to initialize with new credentials in order to get the language list.
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(true);
            }
            else
            {
                this.StatusText = Properties.Resources.Error_KeyInvalid;
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(false);
                return;
            }
            if (! await TranslationServices.Core.TranslationServiceFacade.IsCategoryValidAsync(this.categoryID))
            {
                StatusText = Properties.Resources.Error_CategoryV3Invalid;
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(false);
            }
            return;
          
        }

        #endregion
    }
}