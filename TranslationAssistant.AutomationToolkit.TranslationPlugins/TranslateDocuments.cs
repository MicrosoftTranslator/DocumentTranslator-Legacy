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
            TranslationServiceFacade.Initialize();
            this.sourceDocuments = new SimpleStringArgument(
                "SourceDocuments",
                true,
                new[] { ',' },
                "Full path to the list of documents to translate, or list of documents seperated by comma.");

            this.sourceLanguage = new Argument(
                "SourceLanguage",
                false,
                new[] { "Auto-Detect" },
                TranslationServiceFacade.AvailableLanguages.Values.ToArray(),
                true,
                "The source language. Auto-detect if no language specified.");

            this.targetLanguages = new SimpleStringArgument(
                "TargetLanguages",
                true,
                new string[] { },
                TranslationServiceFacade.AvailableLanguages.Values.ToArray(),
                new[] { ',' },
                "The target language code, or comma-separated list of codes.");

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
                return "Allows to translate the document(s) from source to target language(s).";
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
            if (this.sourceDocuments.Values.ToArray().Any(file => !File.Exists(file.ToString())))
            {
                this.Logger.WriteLine(LogLevel.Error, "Source path does not exist.");
                return false;
            }

            try
            {
                var model = new CommentTranslationModel
                                {
                                    SourceLanguage =
                                        this.sourceLanguage.ValueString ?? "Auto-Detect",
                                    TargetLanguage = this.targetLanguages.ValueString
                                };

                foreach (var file in this.sourceDocuments.ValueString.Split(','))
                {
                    foreach (var language in this.targetLanguages.Values)
                    {
                        try
                        {
                            this.Logger.WriteLine(
                                LogLevel.Msg,
                                string.Format(
                                    "Trying to translate document with name {0} to language {1}.",
                                    file,
                                    language));
                            model.TargetPath = file;
                            this.Logger.WriteLine(
                                LogLevel.Msg,
                                string.Format("Processing file:  {0}.", Path.GetFileName(model.TargetPath)));

                            var sourceLanguageExpanded = String.IsNullOrEmpty(this.sourceLanguage.ValueString)
                                                         || this.sourceLanguage.ValueString.Equals("Auto-Detect")
                                                             ? "Auto-Detect"
                                                             : TranslationServiceFacade.AvailableLanguages[
                                                                 this.sourceLanguage.ValueString];
                            DocumentTranslationManager.DoTranslation(
                                file,
                                false,
                                sourceLanguageExpanded,
                                //TranslationServiceFacade.AvailableLanguages[language.ToString()]);
                                language.ToString());
                            this.Logger.WriteLine(
                                LogLevel.Msg,
                                string.Format(
                                    "******Translated document name {0} to language {1}.*********",
                                    file,
                                    language));
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

            this.Logger.WriteLine(LogLevel.Msg, string.Format("All documents translated successfully."));
            return true;
        }

        #endregion
    }
}