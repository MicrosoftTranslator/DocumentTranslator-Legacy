
namespace TranslationAssistant.AutomationToolkit.TranslationPlugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using TranslationAssistant.AutomationToolkit.BasePlugin;
    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
    using TranslationAssistant.TranslationServices.Core;

    internal class TranslateXML : BasePlugIn
    {
       #region Fields

        /// <summary>
        ///     The source document.
        /// </summary>
        private readonly Argument xmltotranslate;

        /// <summary>
        ///     The source language.
        /// </summary>
        private readonly Argument elementsdispositioncsv;

        /// <summary>
        /// Boolean to indicate if we should generate a list of elements
        /// </summary>
        private readonly Argument generatecsv;

        private readonly Argument fromlanguage;
        private readonly Argument tolanguage;


        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the TranslateXML class.
        /// </summary>
        /// <param name="Logger">
        ///     The logger.
        /// </param>
        public TranslateXML (ConsoleLogger Logger)
            : base(Logger)
        {

            TranslationServiceFacade.Initialize();
            if (!TranslationServiceFacade.IsTranslationServiceReady())
            {
                this.Logger.WriteLine(LogLevel.Error, "Invalid translation service credentials. Use \"DocumentTranslatorCmd setcredentials\", or use the Document Translator Settings option.");
                return;
            }
            
            this.xmltotranslate = new Argument(
                "XML",
                true,
                "The XML file in need of translation");

            this.elementsdispositioncsv = new Argument( 
                "Elements",
                true,
                "CSV file listing the elements to translate");

            this.fromlanguage = new Argument(
                "from",
                false,
                new[] { "Auto-Detect" },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                true,
                "The source language. Auto-detect if no language specified.");

            this.tolanguage = new SimpleStringArgument(
                "to",
                true,
                new string[] { },
                TranslationServiceFacade.AvailableLanguages.Keys.ToArray(),
                new[] { ',' },
                "The target language code, or comma-separated list of language codes.");

            this.generatecsv = new BooleanArgument(
                "generate",
                false,
                false,
                "Set to true if you want to generate a list of elements.");

            this.Arguments = new ArgumentList(
                new[] { this.xmltotranslate, this.elementsdispositioncsv, this.fromlanguage, this.tolanguage, this.generatecsv},
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
                return "Translates XML or generates a default elements file.";
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TranslateXML";
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
            if (generatecsv.ValueString.ToLowerInvariant() == "true")
            {
                try
                {
                    TranslationAssistant.Business.XMLTranslationManager.SaveElementsToCSV(xmltotranslate.ValueString, elementsdispositioncsv.ValueString);
                }
                catch (Exception ex)
                {
                    this.Logger.WriteException(ex);
                    Console.ReadLine();
                    return false;
                }

                this.Logger.WriteLine(LogLevel.Msg, "Element list written to {0}.", elementsdispositioncsv.ValueString);
            }
            else
            {
                int count = TranslationAssistant.Business.XMLTranslationManager.DoTranslation(xmltotranslate.ValueString, elementsdispositioncsv.ValueString, fromlanguage.ValueString, tolanguage.ValueString);
                this.Logger.WriteLine(LogLevel.Msg, "{0} elements translated.", count);
            }

            return true;
        }

        #endregion
    }
}