using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MTLWB.MTService
{
    /// <summary>
    /// Abstract factory class providing interface to interact with the desired translation service.
    /// </summary>
    public abstract class Translator
    {
        internal Translator()
        {
        }


        /// <summary>
        /// Static factory method. Returns the instance of a translator.
        /// </summary>
        /// <returns>A new instance of translator</returns>
        public static Translator GetTranslator()
        {
            return new PublicTranslator();
        }


        /// <summary>
        /// Static factory method. Returns the instance of a translator for the specified service URL.
        /// </summary>
        /// <param name="serviceUrl">The URL of MT service to be used.</param>
        /// <returns>A new instance of translator</returns>
        public static Translator GetTranslator(string serviceUrl)
        {
            return new PublicTranslator(serviceUrl);
        }

        /// <summary>
        /// Bing Application ID to access the desired MT service.
        /// </summary>
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the domain ("general" or "tech") to be used.
        /// </summary>
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// Customization ID.
        /// </summary>
        public string CustomizationId
        {
            get;
            set;
        }
        /// <summary>
        /// Returns an array of strings containing the language codes. 
        /// </summary>
        /// <returns>An array containing the language codes.</returns>
        public abstract string[] GetLanguageCodes();

        /// <summary>
        /// Returns an array of strings containing the language names for the specified language codes
        /// </summary>
        /// <param name="langIds">An array of strings containing the language codes for which the language names are to be retrieved.</param>
        /// <returns>An array containing the language names.</returns>
        public abstract string[] GetLanguageNames(string[] langIds);

        /// <summary>
        /// Translates the specified text and returns the translated text. Abstract method. Implemented in child translator classes.
        /// </summary>
        /// <param name="sourceLanguageId">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguageId">A string representing the language code to translate the text into.</param>
        /// <param name="text">Text to be translated.</param>
        /// <returns>Translated text.</returns>
        public abstract string Translate(string sourceLanguageId, string targetLanguageId, string text);

        /// <summary>
        /// Translates an array of string and returns the translated array. Abstract method. Implemented in child translator classes.
        /// </summary>
        /// <param name="sourceLanguageId">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguageId">A string representing the language code to translate the text into.</param>
        /// <param name="text">Array of string to be translated.</param>
        /// <param name="untranslatedLines">Out parameter containing the list of indices of the strings that failed to translate.</param>
        /// <returns>An array of translated strings</returns>
        public abstract string[] Translate(string sourceLanguageId, string targetLanguageId, string[] text, out List<int> untranslatedLines);
    }
}
