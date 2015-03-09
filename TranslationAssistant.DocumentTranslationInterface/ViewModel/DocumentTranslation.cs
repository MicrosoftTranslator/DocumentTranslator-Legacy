// // ----------------------------------------------------------------------
// // <copyright file="DocumentTranslation.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>DocumentTranslation.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.ViewModel
{
    #region

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Win32;

    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
    using TranslationAssistant.DocumentTranslationInterface.Common;
    using TranslationAssistant.TranslationServices.Core;

    #endregion

    public class DocumentTranslation : Notifyer
    {
        #region Fields

        private bool _isNavigateToTargetFolderEnabled;

        /// <summary>
        ///     The file browse click command
        /// </summary>
        private ICommand inputFilePathBrowseClickCommand;

        /// <summary>
        ///     The input file path.
        /// </summary>
        private List<string> inputFilePaths = new List<string>();

        /// <summary>
        ///     The is go button enabled.
        /// </summary>
        private bool isGoButtonEnabled;

        /// <summary>
        ///     The is started.
        /// </summary>
        private bool isStarted;

        /// <summary>
        ///     The selected source language.
        /// </summary>
        private string selectedSourceLanguage;

        /// <summary>
        ///     The selected target language.
        /// </summary>
        private string selectedTargetLanguage;

        /// <summary>
        ///     The show progress bar.
        /// </summary>
        private bool showProgressBar;

        /// <summary>
        ///     The source language list.
        /// </summary>
        private List<string> sourceLanguageList = new List<string>();

        /// <summary>
        ///     The status text.
        /// </summary>
        private string statusText;

        private string targetFolder;

        private ICommand targetFolderNavigateClickCommand;

        /// <summary>
        ///     The target language list.
        /// </summary>
        private List<string> targetLanguageList = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentTranslation" /> class.
        /// </summary>
        public DocumentTranslation()
        {
            TranslationServiceFacade.Initialize();
            this.PopulateAvailableSourceLanguages();
            this.PopulateAvailableTargetLanguages();
            this.ShowProgressBar = false;
            this.IsGoButtonEnabled = false;
            this.TargetFolder = string.Empty;
            this.StatusText = "Please select file(s) to translate... ";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the go button click command.
        /// </summary>
        public DelegateCommand GoButtonClickCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                        {
                            this.ShowProgressBar = true;
                            this.IsGoButtonEnabled = false;
                            this.StatusText = "Started";

                            var worker = new BackgroundWorker();
                            var model = new CommentTranslationModel
                                            {
                                                SourceLanguage = this.SelectedSourceLanguage,
                                                TargetLanguage = this.SelectedTargetLanguage
                                            };

                            worker.DoWork += (o, e) =>
                                {
                                    this.IsStarted = true;

                                    foreach (var file in this.InputFilePaths)
                                    {
                                        this.TargetFolder = Path.GetDirectoryName(file);
                                        this.IsNavigateToTargetFolderEnabled = true;
                                        model.TargetPath = file;
                                        this.StatusText = "Processing file: " + Path.GetFileName(model.TargetPath);
                                        DocumentTranslationManager.DoTranslation(
                                            file,
                                            false,
                                            this.SelectedSourceLanguage,
                                            this.SelectedTargetLanguage);
                                    }
                                };

                            worker.RunWorkerCompleted += (o, e) =>
                                {
                                    if (e.Error != null)
                                    {
                                        this.StatusText = "Error while processing file: " + model.TargetPath + " "
                                                          + e.Error.Message;
                                    }
                                    else
                                    {
                                        this.StatusText = "Translation completed successfully.";
                                    }

                                    this.IsStarted = false;
                                    this.IsGoButtonEnabled = true;
                                    this.ShowProgressBar = false;
                                };
                            worker.WorkerReportsProgress = false;
                            worker.RunWorkerAsync();
                        },
                    () => true);
            }
        }

        /// <summary>
        ///     Gets the input file path browse click command.
        /// </summary>
        public ICommand InputFilePathBrowseClickCommand
        {
            get
            {
                return this.inputFilePathBrowseClickCommand
                       ?? (this.inputFilePathBrowseClickCommand = new DelegateCommand(this.FilePathBrowseClick));
            }
        }

        /// <summary>
        ///     Gets or sets the input file path.
        /// </summary>
        public List<string> InputFilePaths
        {
            get
            {
                return this.inputFilePaths;
            }

            set
            {
                this.inputFilePaths = value;
                this.IsGoButtonEnabled = this.InputFilePaths.Count > 0;
                this.NotifyPropertyChanged("InputFilePaths");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is go button enabled.
        /// </summary>
        public bool IsGoButtonEnabled
        {
            get
            {
                return this.isGoButtonEnabled;
            }

            set
            {
                this.isGoButtonEnabled = value;
                this.NotifyPropertyChanged("IsGoButtonEnabled");
            }
        }

        public bool IsNavigateToTargetFolderEnabled
        {
            get
            {
                return this._isNavigateToTargetFolderEnabled;
            }
            set
            {
                this._isNavigateToTargetFolderEnabled = value;
                this.NotifyPropertyChanged("IsNavigateToTargetFolderEnabled");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is started.
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }

            set
            {
                this.isStarted = value;
                this.NotifyPropertyChanged("IsStarted");
            }
        }

        /// <summary>
        ///     Gets or sets the selected source language.
        /// </summary>
        public string SelectedSourceLanguage
        {
            get
            {
                return this.selectedSourceLanguage;
            }

            set
            {
                this.selectedSourceLanguage = value;
                this.NotifyPropertyChanged("SelectedSourceLanguage");
            }
        }

        /// <summary>
        ///     Gets or sets the selected target language.
        /// </summary>
        public string SelectedTargetLanguage
        {
            get
            {
                return this.selectedTargetLanguage;
            }

            set
            {
                this.selectedTargetLanguage = value;
                this.NotifyPropertyChanged("SelectedTargetLanguage");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether show progress bar.
        /// </summary>
        public bool ShowProgressBar
        {
            get
            {
                return this.showProgressBar;
            }

            set
            {
                this.showProgressBar = value;
                this.NotifyPropertyChanged("ShowProgressBar");
            }
        }

        /// <summary>
        ///     Gets or sets the source language list.
        /// </summary>
        public List<string> SourceLanguageList
        {
            get
            {
                return this.sourceLanguageList;
            }

            set
            {
                this.sourceLanguageList = value;
                this.NotifyPropertyChanged("SourceLanguageList");
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

        public string TargetFolder
        {
            get
            {
                return this.targetFolder;
            }

            set
            {
                this.targetFolder = value;
                this.NotifyPropertyChanged("TargetFolder");
            }
        }

        public ICommand TargetFolderNavigateClickCommand
        {
            get
            {
                return this.targetFolderNavigateClickCommand
                       ?? (this.targetFolderNavigateClickCommand = new DelegateCommand(this.NavigateToTargetFolderClick));
            }
        }

        /// <summary>
        ///     Gets or sets the target language list.
        /// </summary>
        public List<string> TargetLanguageList
        {
            get
            {
                return this.targetLanguageList;
            }

            set
            {
                this.targetLanguageList = value;
                this.NotifyPropertyChanged("TargetLanguageList");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Assemblies the browse_ click.
        /// </summary>
        private void FilePathBrowseClick()
        {
            var openfileDlg = new OpenFileDialog
                                  {
                                      Filter =
                                          "Supported Files|*.doc; *.docx; *.pdf; *.xls; *.xlsx; *.ppt; *.pptx; *.htm; *.html",
                                      Multiselect = true
                                  };
            if (openfileDlg.ShowDialog().Value)
            {
                this.InputFilePaths = openfileDlg.FileNames.ToList();
            }
        }

        private void NavigateToTargetFolderClick()
        {
            if (!string.IsNullOrEmpty(this.TargetFolder))
            {
                Process.Start(new ProcessStartInfo(this.TargetFolder));
            }
        }

        /// <summary>
        ///     The pouplate available source languages.
        /// </summary>
        private void PopulateAvailableSourceLanguages()
        {
            this.SourceLanguageList.Add("Auto-Detect");
            this.SourceLanguageList.AddRange(TranslationServiceFacade.AvailableLanguages.Values);
        }

        /// <summary>
        ///     The pouplate available target languages.
        /// </summary>
        private void PopulateAvailableTargetLanguages()
        {
            this.TargetLanguageList.AddRange(TranslationServiceFacade.AvailableLanguages.Values);
        }

        #endregion
    }
}