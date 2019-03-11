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


    #endregion

    /// <summary>
    ///     Interaction logic for ProjectCodeCommentsTranslationPage.xaml
    /// </summary>
    public partial class ImmediateWindow : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentTranslationPage" /> class.
        /// </summary>
        public ImmediateWindow()
        {
            this.InitializeComponent();
            this.KeyDown += ImmediateWindow_KeyDown;
        }

        private void ImmediateWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.F12:
                        break;
                }
            }
            catch (System.Exception ex) { };
        }

        #endregion

    }
}