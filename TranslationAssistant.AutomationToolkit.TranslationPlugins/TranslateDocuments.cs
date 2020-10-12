// // ----------------------------------------------------------------------
// // <copyright file="TranslateDocuments.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>TranslateDocuments.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.TranslationPlugins
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    using TranslationAssistant.AutomationToolkit.BasePlugin;
    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
    using TranslationAssistant.TranslationServices.Core;

    /// <summary>
    ///     The translate documents.
    /// </summary>
    internal class TranslateDocuments : BasePlugIn
    {
        #region Fields

        /// <summary>
        ///     The source document.
        /// </summary>
        private readonly Argument sourceDocuments;

        /// <summary>
        ///     The source language.
        /// </summary>
        private readonly Argument sourceLanguage;

        /// <summary>
        ///     The target language.
        /// </summary>
        private readonly Argument targetLanguages;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslateDocuments" /> class.
        /// </summary>
        /// <param name="Logger">
        ///     The logger.
        /// </param>
        public TranslateDocuments(ConsoleLogger Logger)
            : base(Logger)
        {
            try
            {
                _ = TranslationServiceFacade.Initialize();
            }
            catch (CredentialsMissingException ex)
            {
                this.Logger.WriteLine(LogLevel.Error, ex.Message);
            }
            if (!TranslationServiceFacade.IsTranslationServiceReady())
            {
                this.Logger.WriteLine(LogLevel.Error, "Invalid translation service credentials. Use \"DocumentTranslatorCmd setcredentials\"");
                return;
            }

            this.sourceDocuments = new SimpleStringArgument(
                "Documents",
                true,
                new[] { ',' },
                "Document to translate, or list of documents separated by comma, or a wildcard. Wildcard recurses through subfolders.");

            this.sourceLanguage = new Argument(
                "from",
                false,
                new[] { "Auto-Detect" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The source language. Auto-detect if no language specified.");

            this.targetLanguages = new SimpleStringArgument(
                "to",
                true,
                new string[] { },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                new[] { ',' },
                "The target language code, or comma-separated list of language codes.");

            this.Arguments = new ArgumentList(
                new[] { this.sourceDocuments, this.sourceLanguage, this.targetLanguages },
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
                return "Allows to translate a list of documents from a source language to a list of target languages.";
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TranslateDocuments";
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
            int documentcount = 0;
            List<String> listoffiles = new List<string>();

            //Expand wildcard, if name specification contains *
            if (this.sourceDocuments.Values.ToArray().Any(file => file.ToString().Contains("*"))) { 
                foreach (string filename in this.sourceDocuments.Values.ToArray())
                {
                    int lastBackslashPosition = filename.LastIndexOf('\\') + 1;
                    string path = filename.Substring(0, lastBackslashPosition);
                    string filenameOnly = filename.Substring(lastBackslashPosition);
                    String[] filelist= Directory.GetFiles(path, filenameOnly, SearchOption.AllDirectories);
                    listoffiles.AddRange(filelist);
                }
            }
            else  //no * in the file name
            {
                foreach (var file in this.sourceDocuments.ValueString.Split(','))
                {
                    listoffiles.Add(file);
                }
            }

            try
            {
                var model = new CommentTranslationModel
                                {
                                    SourceLanguage =
                                        this.sourceLanguage.ValueString ?? "Auto-Detect",
                                    TargetLanguage = this.targetLanguages.ValueString
                                };

                foreach (var file in listoffiles)
                {
                    if (!File.Exists(file)) Logger.WriteLine(LogLevel.Error, String.Format("Specified document {0} does not exist. ", file));
                    foreach (var language in this.targetLanguages.Values)
                    {
                        try
                        {
                            this.Logger.WriteLine(
                                LogLevel.Msg,
                                string.Format(
                                    "Translating document {0} to language {1}.",
                                    file,
                                    language));
                            model.TargetPath = file;
                            var sourceLanguageExpanded = String.IsNullOrEmpty(this.sourceLanguage.ValueString)
                                                         || this.sourceLanguage.ValueString.Equals("Auto-Detect")
                                                             ? "Auto-Detect"
                                                             : TranslationServiceFacade.AvailableLanguages[
                                                                 this.sourceLanguage.ValueString];
                            string languagename = TranslationServiceFacade.LanguageCodeToLanguageName(language.ToString());

                            DocumentTranslationManager.DoTranslation(
                                file,
                                false,
                                sourceLanguageExpanded,
                                languagename);
                            this.Logger.WriteLine(
                                LogLevel.Msg,
                                string.Format(
                                    "-- Translated document name {0} to language {1}.",
                                    file,
                                    language));
                            documentcount++;
                        }
                        catch (Exception ex)
                        {
                            this.Logger.WriteLine(
                                LogLevel.Error,
                                string.Format(
                                    "Error while processing file: {0} to language {1} with error: {2}",
                                    model.TargetPath,
                                    language,
                                    ex.Message));
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.WriteException(ex);
                Console.ReadLine();
                return false;
            }

            this.Logger.WriteLine(LogLevel.Msg, string.Format("Documents translated successfully: {0}.", documentcount));
            return true;
        }

        #endregion
    }
}