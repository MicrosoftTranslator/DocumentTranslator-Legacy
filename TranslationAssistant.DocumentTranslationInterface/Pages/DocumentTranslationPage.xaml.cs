// // ----------------------------------------------------------------------
// // <copyright file="DocumentTranslationPage.xaml.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>DocumentTranslationPage.xaml.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.Pages
{
    #region Usings

    using System.Windows.Controls;
    using TranslationAssistant.DocumentTranslationInterface.Common;


    #endregion

    /// <summary>
    ///     Interaction logic for ProjectCodeCommentsTranslationPage.xaml
    /// </summary>
    public partial class DocumentTranslationPage : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentTranslationPage" /> class.
        /// </summary>
        public DocumentTranslationPage()
        {
            this.InitializeComponent();
            SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Unsubscribe(RefreshLanguageComboBoxes);
            SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Subscribe(RefreshLanguageComboBoxes);
            this.KeyDown += DocumentTranslationPage_KeyDown;
        }

        private void DocumentTranslationPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.F11:
                        if (TranslationServices.Core.TranslationServiceFacade.CreateTMXOnTranslate)
                        {
                            TranslationServices.Core.TranslationServiceFacade.CreateTMXOnTranslate = false;
                        }
                        else
                        {
                            TranslationServices.Core.TranslationServiceFacade.CreateTMXOnTranslate = true;
                        }
                        break;
                }
            }
            catch (System.Exception) { };
        }

        private void RefreshLanguageComboBoxes(bool successful)
        {
            cbSourceLanguages.Items.Refresh();
            cbTargetLanguages.Items.Refresh();
        }

        #endregion

    }
}