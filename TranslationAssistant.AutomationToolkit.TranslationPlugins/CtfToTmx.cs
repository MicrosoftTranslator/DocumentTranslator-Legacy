//Command line plugin for copying your CTF content into a TMX.
//Creates a TMX and CSV file (same content) of the content.
//CTF is tightly integrated with the MT system, which works sentence based.

using System.Threading;

namespace TranslationAssistant.AutomationToolkit.TranslationPlugins
{
    using Mts.Common.Tmx;
    using System;
    using System.IO;
    using System.Linq;
    using TranslationAssistant.AutomationToolkit.BasePlugin;
    using TranslationAssistant.TranslationServices.Core;

    internal class CtfToTmx : BasePlugIn
    {
        #region Fields

        /// <summary>
        ///     The source document.
        /// </summary>
        private readonly Argument TmxDocument;

        /// <summary>
        ///     The source language.
        /// </summary>
        private readonly Argument sourceLanguage;

        /// <summary>
        ///     The target language.
        /// </summary>
        private readonly Argument targetLanguage;

        /// <summary>
        ///     The user for the CTF record.
        /// </summary>
        private readonly Argument user;

        /// <summary>
        ///     The rating for the CTF record.
        /// </summary>
        private readonly Argument rating;

        #endregion

        #region Constructors and Destructors

        public CtfToTmx(ConsoleLogger Logger)
            : base(Logger)
        {
            TranslationServiceFacade.Initialize();
            if (!TranslationServiceFacade.IsTranslationServiceReady())
            {
                this.Logger.WriteLine(LogLevel.Error, "Invalid translation service credentials. Use \"DocumentTranslatorCmd setcredentials\", or use the Document Translator Settings option.");
                return;
            }

            this.TmxDocument = new SimpleStringArgument(
                "Tmx",
                true,
                new[] { ',' },
                "TMX Document to create.");

            this.sourceLanguage = new Argument(
                "From",
                false,
                new[] { "" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The source language code. Must be a valid Microsoft Translator language code, and the same as language code used in the CTF store. Mapped to TMX language code in TmxLangMap.csv.");

            this.targetLanguage = new Argument(
                "To",
                false,
                new[] { "" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The target language code. Must be a valid Microsoft Translator language code, and the same as language code used in the CTF store. Mapped to TMX language code in TmxLangMap.csv.");
            
            this.user = new Argument(
                "User",
                false,
                "If provided, filter by the given user. The default user name for data uploaded using this tool is TmxUpload. Download is not filtered by default.");

            this.rating = new Argument(
                "Rating",
                false,
                "The download can be filtered by rating. Default is no filtering.");

            this.Arguments = new ArgumentList(
                new[] { this.TmxDocument, this.sourceLanguage, this.targetLanguage, this.user, this.rating },
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
                return "Downloads the content of your CTF store into a TMX of your choice.";
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "CtfToTmx";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute method.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Execute()
        {
            int skip = 0;
            int count = 100;
            int totalcount = 0;
            bool hascontent = true;
            
            TranslationServiceFacade.UserTranslation[] usertranslations = new TranslationServiceFacade.UserTranslation[100];
            usertranslations.Initialize();

            using (TmxWriter TmxDocument = new TmxWriter(this.TmxDocument.ValueString, this.sourceLanguage.ValueString, this.targetLanguage.ValueString))
            {
                do
                {
                    usertranslations = TranslationServiceFacade.GetUserTranslations(this.sourceLanguage.ValueString, this.targetLanguage.ValueString, skip, count);
                    skip += count;
                    foreach (var usertrans in usertranslations)
                    {
                        if (usertrans.OriginalText == null)
                        {
                            hascontent = false;
                            break;
                        }
                        TmxDocument.TmxWriteSegment(usertrans.OriginalText, usertrans.TranslatedText, usertrans.From, usertrans.To, TmxWriter.TUError.good);
                        totalcount++;
                    }
                    Logger.WriteLine(LogLevel.None, "{0} translation units written.", totalcount);
                } while (hascontent);
            } 


            return true;
        }
        #endregion
    }
}
