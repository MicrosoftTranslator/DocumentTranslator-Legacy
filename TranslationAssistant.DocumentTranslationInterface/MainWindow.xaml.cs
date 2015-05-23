// // ----------------------------------------------------------------------
// // <copyright file="MainWindow.xaml.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>MainWindow.xaml.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface
{
    #region

    using FirstFloor.ModernUI.Windows.Controls;

    #endregion

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.FileToProcess = string.Empty;
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public string FileToProcess { get; set; }
        public string ProjectToProcess { get; set; }
        public string SolutionToProcess { get; set; }

        public bool IsTranslationServiceReady { get; set; }

        #endregion
    }
}