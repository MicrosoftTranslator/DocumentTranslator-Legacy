//Command line plugin for importing a TMX into Microsoft Translator CTF.
//Creates a TMX and CSV file (same content) of the errors.
//System rejects all segments that have an unequal number of sentences, have tags, or a large delta in lengths
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

    internal class TmxToCtf : BasePlugIn
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

        /// <summary>
        /// Write to CTF or just list the records that would be written.
        /// </summary>
        private readonly BooleanArgument boolWrite;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TmxToCtf" /> class.
        /// </summary>
        /// <param name="Logger">
        ///     The logger.
        /// </param>
        public TmxToCtf(ConsoleLogger Logger)
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
                "TMX Document to upload to CTF.");

            this.sourceLanguage = new Argument(
                "From",
                true,
                new[] { "Auto-Detect" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The source language code. Must be a valid Microsoft Translator language code, and the same as language code used in the TMX, or mapped via TmxLangMap.csv (from, to).");

            this.targetLanguage = new Argument(
                "To",
                true,
                new string[] { "de" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The target language code. Must be a valid Microsoft Translator language code, and the same as language code used in the TMX, or mapped via TmxLangMap.csv (from, to).");
            
            this.user = new Argument(
                "User",
                false,
                "The user ID recorded in CTF. Default: TmxUpload.");

            this.rating = new Argument(
                "Rating",
                false,
                "The rating with which the entries are created. Must be an integer 1..10. 5 or higher overrides MT. Default: 6.");

            this.boolWrite = new BooleanArgument(
                "Write",
                false,
                false,
                "Write to CTF.");

            this.Arguments = new ArgumentList(
                new[] { this.TmxDocument, this.sourceLanguage, this.targetLanguage, this.user, this.rating, this.boolWrite },
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
                return "Uploads the content of a TMX file into the Collaborative Translation Framework (CTF), the built-in translation memory of Microsoft Translator. The segments of the TMX will be used in subsequent translation jobs using the same client ID.";
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TmxToCtf";
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
            string SntFileName = Path.GetTempPath() + "_TmxUpload.snt";
            string uservalue = user.ValueString;
            if (uservalue == string.Empty) uservalue = "TmxUpload";
            string ratingvalue = rating.ValueString;
            if (ratingvalue == string.Empty) ratingvalue = "6";
            if (!File.Exists(TmxDocument.ValueString))
            {
                Logger.WriteLine(LogLevel.Error, "File {0} does not exist.", TmxDocument.ValueString);
                return false;
            }

            TmxFile TmxIn = new TmxFile(this.TmxDocument.ValueString);
            string[] sntFilenames = TmxIn.WriteToSNTFiles(SntFileName);
            if (sntFilenames != null)
            {
                if (sntFilenames.Length != 2)
                {
                    Logger.WriteLine(LogLevel.Error, "More than 2 languages in the TMX file. Must have exactly 2.");
                    deleteSNTfiles(sntFilenames);
                    return false;
                }
            }
            else
            {
                Logger.WriteLine(LogLevel.Error, "Not a TMX file. Aborting.");
                return false;
            }
            TranslationMemory TM = new TranslationMemory();
            TM.sourceLangID = this.sourceLanguage.ValueString.ToLowerInvariant();
            TM.targetLangID = this.targetLanguage.ValueString.ToLowerInvariant();


            // Read language names from Tmx
            string TmxSourceLanguage = Path.GetFileNameWithoutExtension(sntFilenames[0]);
            TmxSourceLanguage = TmxSourceLanguage.Substring(TmxSourceLanguage.LastIndexOf('_') + 1).ToLowerInvariant();
            string TmxTargetLanguage = Path.GetFileNameWithoutExtension(sntFilenames[1]);
            TmxTargetLanguage = TmxTargetLanguage.Substring(TmxTargetLanguage.LastIndexOf('_') + 1).ToLowerInvariant();

            if (TmxSourceLanguage.Substring(0, 2) != TM.sourceLangID)
            {
                Logger.WriteLine(LogLevel.Error, "Source language mismatch between command line {0} and TMX language {1}. Please edit TmxLangMap.csv to fix. Aborting.", TM.sourceLangID, TmxSourceLanguage);
                deleteSNTfiles(sntFilenames);
                return false;
            }

            if (TmxTargetLanguage.Substring(0, 2) != TM.targetLangID)
            {
                Logger.WriteLine(LogLevel.Error, "Target language mismatch between command line {0} and TMX language {1}. Please edit TmxLangMap.csv to fix. Aborting.", TM.targetLangID, TmxTargetLanguage);
                deleteSNTfiles(sntFilenames);
                return false;
            }

            TranslationMemory TMErrors = new TranslationMemory();
            TMErrors.sourceLangID = this.sourceLanguage.ValueString.ToLowerInvariant();
            TMErrors.targetLangID = this.targetLanguage.ValueString.ToLowerInvariant();


            string[] sntSource = File.ReadAllLines(sntFilenames[0]);
            string[] sntTarget = File.ReadAllLines(sntFilenames[1]);
            if (sntSource.Length != sntTarget.Length){
                Logger.WriteLine(LogLevel.Error, "Unequal number of segments. The TMX must have the same number of segments in the two given languages.");
                deleteSNTfiles(sntFilenames);
                return false;
            }

            TmxWriter ErrorTmx = new TmxWriter(Path.GetFileNameWithoutExtension(this.TmxDocument.ValueString) + ".errors." + TmxSourceLanguage + "_" + TmxTargetLanguage + "." + DateTime.Now.ToString("yyyyMMddThhmmssZ") + ".tmx", TmxSourceLanguage, TmxTargetLanguage, false);

            //Load into TM without error checks. The AddTranslationSegments method performs error checks. 
            for (int sntLineIndex = 0; sntLineIndex < sntSource.Length; sntLineIndex++)
            {
                TranslationUnit TU = new TranslationUnit(sntSource[sntLineIndex], sntTarget[sntLineIndex], Convert.ToInt16(ratingvalue), uservalue, "", TUStatus.good, "");
                TM.Add(TU);
            }

            Logger.WriteLine(LogLevel.None, "{0} translation units read.", sntSource.Length);

            
            bool AddToCTF = false;
            if (boolWrite.ValueString.ToLowerInvariant() == "true") AddToCTF = true;
            TMFunctions.AddTranslationSegmentsResponse ATSResponse = TMFunctions.AddTranslationSegments(TM, TMErrors, TM.sourceLangID, TM.targetLangID, Convert.ToInt16(ratingvalue), uservalue, AddToCTF);

            Logger.WriteLine(LogLevel.Msg, "Error segments: {0}", ATSResponse.errorsegments);
           
            if (AddToCTF)
            {
                Logger.WriteLine(LogLevel.Msg, "{0} sentences written to CTF. Write complete. ", ATSResponse.sentencesadded);
            }
            else
            {
                Logger.WriteLine(LogLevel.None, "Nothing written to CTF.");
            }


            return true;
        }

        private void deleteSNTfiles(string[] sntFilenames)
        {
            foreach (string filename in sntFilenames)
            {
                File.Delete(filename);
            }
        }

        #endregion
    }
}
