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
    using System;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;

    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
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
        private string clientID;

        /// <summary>
        ///     The client secret.
        /// </summary>
        private string clientSecret;

        /// <summary>
        ///     The category identifier.
        /// </summary>
        private string categoryID;

        /// <summary>
        ///     The save account settings click command.
        /// </summary>
        private ICommand saveAccountSettingsClickCommand;

        /// <summary>
        ///     The status text.
        /// </summary>
        private string statusText;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountViewModel" /> class.
        /// </summary>
        public AccountViewModel()
        {
            this.StatusText = string.Empty;
            AccountModel settings = new AccountManager().GetAccountSettings();
            this.clientSecret = settings.ClientSecret;
            this.clientID = settings.ClientID;
            this.categoryID = settings.CategoryID;

        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the application id.
        /// </summary>
        public string ClientID
        {
            get
            {
                return this.clientID;
            }

            set
            {
                this.clientID = value;
                this.NotifyPropertyChanged("ClientID");
            }
        }

        /// <summary>
        ///     Gets or sets the client secret.
        /// </summary>
        public string ClientSecret
        {
            get
            {
                return this.clientSecret;
            }

            set
            {
                this.clientSecret = value;
                this.NotifyPropertyChanged("ClientSecret");
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
        ///     Assemblies the browse_ click.
        /// </summary>
        private void SaveAccountClick()
        {
            var model = new AccountModel { ClientSecret = this.ClientSecret, ClientID = this.clientID, CategoryID = this.categoryID};

            try
            {
                new AccountManager().SaveAccountSettings(model);
            }
            catch (Exception ex)
            {
                this.StatusText = ex.Message;
            }

            this.StatusText = "Settings saved.";
        }

        #endregion
    }
}