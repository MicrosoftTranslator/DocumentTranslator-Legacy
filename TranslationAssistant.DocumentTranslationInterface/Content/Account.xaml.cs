// // ----------------------------------------------------------------------
// // <copyright file="Account.xaml.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>Account.xaml.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.Content
{
    #region

    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    #endregion

    /// <summary>
    ///     Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsAppearance" /> class.
        /// </summary>
        public Account()
        {
            this.InitializeComponent();
            KeyBox.Password = TranslationServices.Core.TranslationServiceFacade.AzureKey;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            TranslationServices.Core.TranslationServiceFacade.AzureKey = KeyBox.Password;
        }
        #endregion
    }
}