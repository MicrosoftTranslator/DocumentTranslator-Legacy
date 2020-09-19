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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for ProjectCodeCommentsTranslationPage.xaml
    /// </summary>
    public partial class ImmediateWindow : UserControl
    {
        #region Constructors and Destructors
        readonly ViewModel.DocumentTranslation documentTranslation = new ViewModel.DocumentTranslation();


        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentTranslationPage" /> class.
        /// </summary>
        public ImmediateWindow()
        {
            this.InitializeComponent();
            this.Loaded += ImmediateWindow_Loaded;
            this.GotFocus += ImmediateWindow_GotFocus;
        }

        private void ImmediateWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ImmediateWindow.xaml.cs: Available Languages: {0}", TranslationServices.Core.TranslationServiceFacade.AvailableLanguages.Count);
            documentTranslation.PopulateAvailableLanguages();
            cbSourceLanguages.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateTarget();
            cbTargetLanguages.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateTarget();
            
        }

        private void ImmediateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += Window_Closing;
        }

        #endregion

        private async void DetectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Task<string> task = TranslationServices.Core.TranslationServiceFacade.DetectAsync(InputBox.Text, false);
            ResultBox.Text = string.Empty;
            ResultBox.Text = await task;
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResultBox.Text = string.Empty;
            string translateFrom = documentTranslation.SelectedSourceLanguage;
            TranslationServices.Core.TranslationServiceFacade.ContentType contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.plain;
            if (documentTranslation.SourceLanguageList.IndexOf(documentTranslation.SelectedSourceLanguage) == 0)
            {
                translateFrom = "";
            }
            else
            {
                translateFrom = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(translateFrom);
            }

            if (documentTranslation.TranslateModeList.IndexOf(documentTranslation.SelectedTranslateMode) == 1) contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.HTML;

            Task<string> task = TranslationServices.Core.TranslationServiceFacade.TranslateStringAsync(
                InputBox.Text,
                translateFrom,
                TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(documentTranslation.SelectedTargetLanguage),
                contentType
            );
            try
            {
                ResultBox.Text = await task;
            }
            catch (Exception ex)
            {
                ResultBox.Text = ex.Message;
            }
        }

        private async void BreakSentencesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResultBox.Text = string.Empty;
            string language = documentTranslation.SelectedSourceLanguage;
            if (documentTranslation.SourceLanguageList.IndexOf(language) == 0) language = string.Empty;
            else language = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(language);
            Task<List<int>> task = TranslationServices.Core.TranslationServiceFacade.BreakSentencesAsync(InputBox.Text, language);
            StringBuilder outputstring = new StringBuilder(string.Empty);
            int startindex = 0;
            _ = new List<int>();
            List<int> BreakResult;
            try
            {
                BreakResult = await task;
            }
            catch (Exception ex)
            {
                ResultBox.Text = ex.Message;
                return;
            }
            foreach (int offset in BreakResult)
            {
                outputstring.Append(InputBox.Text.Substring(startindex, offset));
                outputstring.AppendLine('['+ offset.ToString() + ']');
                startindex += offset;
            }
            outputstring.Append("\n*** "+ Properties.Resources.Translate_SentNum + ' ' + task.Result.Count + " ***");
            ResultBox.Text = outputstring.ToString();
        }

        private void CbTargetLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            documentTranslation.SelectedTargetLanguage = e.AddedItems[0].ToString();
            if (ResultBox != null)
            {
                if ((TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(documentTranslation.SelectedTargetLanguage) == "he") ||
                 (TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(documentTranslation.SelectedTargetLanguage) == "ar"))
                {
                    ResultBox.HorizontalAlignment = HorizontalAlignment.Right;
                    ResultBox.TextAlignment = TextAlignment.Right;
                    ResultBox.FlowDirection = FlowDirection.RightToLeft;
                    ResultBox.HorizontalContentAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    ResultBox.HorizontalAlignment = HorizontalAlignment.Left;
                    ResultBox.TextAlignment = TextAlignment.Left;
                    ResultBox.FlowDirection = FlowDirection.LeftToRight;
                    ResultBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                }
                ResultBox.InvalidateArrange();
                ResultBox.UpdateLayout();
            }
        }

        private void CbSourceLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            documentTranslation.SelectedSourceLanguage = e.AddedItems[0].ToString();
            if (InputBox != null)
            {
                string langcode = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(documentTranslation.SelectedSourceLanguage);
                if ((langcode == "he") || (langcode == "ar"))   //Bidi languages
                {
                    InputBox.HorizontalAlignment = HorizontalAlignment.Right;
                    InputBox.TextAlignment = TextAlignment.Right;
                    InputBox.FlowDirection = FlowDirection.RightToLeft;
                    InputBox.HorizontalContentAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    InputBox.HorizontalAlignment = HorizontalAlignment.Left;
                    InputBox.TextAlignment = TextAlignment.Left;
                    InputBox.FlowDirection = FlowDirection.LeftToRight;
                    InputBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                }
                ResultBox.InvalidateArrange();
                InputBox.UpdateLayout();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            documentTranslation.SaveSettings();
        }

        private void CbTranslateMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            documentTranslation.SelectedTranslateMode = e.AddedItems[0].ToString();
        }

        private async void TranslateToAllButton_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = string.Empty;
            string translateFrom = documentTranslation.SelectedSourceLanguage;
            TranslationServices.Core.TranslationServiceFacade.ContentType contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.plain;
            if (documentTranslation.SourceLanguageList.IndexOf(documentTranslation.SelectedSourceLanguage) == 0)
            {
                translateFrom = "";
            }
            else
            {
                translateFrom = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(translateFrom);
            }

            if (documentTranslation.TranslateModeList.IndexOf(documentTranslation.SelectedTranslateMode) == 1) contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.HTML;

            Business.TranslateToAll translateToAll = new Business.TranslateToAll();
            translateToAll.OneTranslationDone += TranslateToAll_OneTranslationDone;
            Task<string> task = translateToAll.TranslateToAllLanguagesString
            (
                InputBox.Text,
                translateFrom,
                TranslationServices.Core.TranslationServiceFacade.CategoryID,
                contentType
            );
            try
            {
                ResultBox.Text = await task;
            }
            catch (Exception ex)
            {
                ResultBox.Text = ex.Message;
            }
        }

        /// <summary>
        /// Generate some animation in the Result box, while we are waiting for the translations to come in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TranslateToAll_OneTranslationDone(object sender, EventArgs e)
        {
            ResultBox.Text = ResultBox.Text += ".";
        }

        /// <summary>
        /// Generate some animation in the Result box, while we are waiting for the translations to come in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TranslateFromAll_OneTranslationDone(object sender, EventArgs e)
        {
            ResultBox.Text = ResultBox.Text += ".";
        }


        private async void DictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = string.Empty;
            string translateFrom = documentTranslation.SelectedSourceLanguage;
            if (documentTranslation.SourceLanguageList.IndexOf(documentTranslation.SelectedSourceLanguage) == 0)
            {
                translateFrom = "";
            }
            else
            {
                translateFrom = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(translateFrom);
            }
            string translateTo = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(documentTranslation.SelectedTargetLanguage);

            Task<string> task = TranslationServices.Core.TranslationServiceFacade.Dictionary
            (
                InputBox.Text,
                translateFrom,
                translateTo
            );
            try
            {
                string dictionaryResult = await task;
                using (var stringReader = new StringReader(dictionaryResult))
                using (var stringWriter = new StringWriter())
                {
                    var jsonReader = new JsonTextReader(stringReader);
                    using (var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented })
                    {
                        jsonWriter.WriteToken(jsonReader);
                    }
                    ResultBox.Text = stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                ResultBox.Text = ex.Message;
            }
        }

        private async void TranslateFromAllButton_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = string.Empty;
            string translateTo = documentTranslation.SelectedTargetLanguage;
            TranslationServices.Core.TranslationServiceFacade.ContentType contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.plain;
            if (documentTranslation.SourceLanguageList.IndexOf(documentTranslation.SelectedTargetLanguage) == 0)
            {
                translateTo = "en";
            }
            else
            {
                translateTo = TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(translateTo);
            }

            if (documentTranslation.TranslateModeList.IndexOf(documentTranslation.SelectedTranslateMode) == 1) contentType = TranslationServices.Core.TranslationServiceFacade.ContentType.HTML;

            Business.TranslateFromAll translateFromAll = new Business.TranslateFromAll();
            translateFromAll.OneTranslationDone += TranslateFromAll_OneTranslationDone;
            Task<string> task = translateFromAll.TranslateFromAllLanguagesString
            (
                InputBox.Text,
                translateTo,
                TranslationServices.Core.TranslationServiceFacade.CategoryID,
                contentType
            );
            try
            {
                ResultBox.Text = await task;
            }
            catch (Exception ex)
            {
                ResultBox.Text = ex.Message;
            }
        }

        private async void ListAllLanguagesButton_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = await TranslationAssistant.Business.AllLanguageList.GetAllLanguages();
        }
    }
}