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
    #region Usings
    using System;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    #endregion

    /// <summary>
    ///     Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : UserControl
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsAppearance" /> class.
        /// </summary>
        public Account()
        {
            this.InitializeComponent();
            KeyBox.Password = TranslationServices.Core.TranslationServiceFacade.AzureKey;
            CloudSelector.Text = TranslationServices.Core.TranslationServiceFacade.AzureCloud;
            RegionSelector.Text = TranslationServices.Core.TranslationServiceFacade.AzureRegion;
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

        public event EventHandler ShowExperimental_Changed;

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = TranslationServices.Core.TranslationServiceFacade.Initialize(true);
            ShowExperimental_Changed?.Invoke(this, EventArgs.Empty);
        }
        private void CloudSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void RegionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TranslationServices.Core.TranslationServiceFacade.AzureRegion = RegionSelector.SelectedItem.ToString();
        }

    }
}