using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    using Mts.Common.Tmx;

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
            }

            this.TmxDocument = new SimpleStringArgument(
                "TmxDocument",
                true,
                new[] { ',' },
                "TMX Document to upload to CTF.");

            this.sourceLanguage = new Argument(
                "SourceLanguage",
                true,
                new[] { "Auto-Detect" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The source language code. Must match the language specification in the TMX file AND be a valid Microsoft Translator language code.");

            this.targetLanguage = new SimpleStringArgument(
                "TargetLanguage",
                true,
                new string[] { },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                new[] { ',' },
                "The target language code. Must match the language specification in the TMX file AND be a valid Microsoft Translator language code.");
            
            this.user = new Argument(
                "User",
                false,
                "The user ID recorded in CTF. Default: TmxUpload.");

            this.rating = new SimpleStringArgument(
                "Rating",
                false,
                new[] { ',' },
                "The rating with which the entries are created. Must be an integer 1..10. 5 or higher overrides MT. Default: 6.");

            this.Arguments = new ArgumentList(
                new[] { this.TmxDocument, this.sourceLanguage, this.targetLanguage },
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
            TmxFile TmxIn = new TmxFile(this.TmxDocument.ValueString);
            string[] sntFilenames = TmxIn.WriteToSNTFiles(SntFileName);
            if (sntFilenames.Length != 2) {
                Logger.WriteLine(LogLevel.Error, "More than 2 languages in the TMX file. Must have exactly 2.");
                deleteSNTfiles(sntFilenames);
                return false;
            }

            TranslationMemory TM = new TranslationMemory();
            TM.sourceLangID = this.sourceLanguage.ValueString;
            TM.targetLangID = this.targetLanguage.ValueString;

            string[] sntSource = File.ReadAllLines(sntFilenames[0]);
            string[] sntTarget = File.ReadAllLines(sntFilenames[1]);
            if (sntSource.Length != sntTarget.Length){
                Logger.WriteLine(LogLevel.Error, "Unequal number of segments. The TMX must have the same number of segments in the two given languages.");
                deleteSNTfiles(sntFilenames);
                return false;
            }


            //Load into TM and perform error check on each line.
            int ratioViolationCount = 0; //counts number of ratio violations
            int sntCountViolationCount = 0; //counts number of unequal sentence count violation.
            for (int i = 0; i < sntSource.Length; i++)
            {

                //Length discrepancy check
                float ratio = Math.Abs(sntSource[i].Length / sntTarget.Length);
                if (ratio > 3) //skip the segment
                {
                    Logger.WriteLine(LogLevel.Msg, "Length ratio exceeded. Segment skipped {0}", sntSource[i].Substring(0, 60));
                    ratioViolationCount++;
                    if ((ratioViolationCount / sntSource.Length) > 0.10)
                    {
                        Logger.WriteLine(LogLevel.Error, "Length ratio exceeded for 10% of segments. Probably not a translation. Aborting.");
                        deleteSNTfiles(sntFilenames);
                        return false;
                    }
                    continue;
                }

                int[] sourceSentLengths = TranslationServiceFacade.BreakSentences(sntSource[i], TM.sourceLangID);
                int[] targetSentLengths = TranslationServiceFacade.BreakSentences(sntTarget[i], TM.targetLangID);

                //unequal sentence count violation check
                if (sourceSentLengths.Length != targetSentLengths.Length)
                {
                    sntCountViolationCount++;
                    Logger.WriteLine(LogLevel.Msg, "Unequal number of sentences in segment. Segment skipped {0}", sntSource[i].Substring(0, 60));
                    if ((sntCountViolationCount / sntSource.Length) > 0.10)
                    {
                        Logger.WriteLine(LogLevel.Error, "Unequal sentence count exceeded for 10% of segments. Probably not a translation. Aborting.");
                        deleteSNTfiles(sntFilenames);
                        return false;
                    }
                    continue;
                }

                //Split multiple sentences
                if (sourceSentLengths.Length > 1)
                {
                    
                }

                //normalize tags

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
