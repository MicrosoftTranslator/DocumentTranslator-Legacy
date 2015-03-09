using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using MTLWB.HtmlParser;
using MTLWB.Common.IO;
using MTLWB.Common.Log;
using TranslationAssistant.TranslationServices.Core;

namespace MTLWB.Common
{
    /// <summary>
    /// Represents a supported language
    /// </summary>
    public struct Language
    {
        /// <summary>
        /// Language abbreviation (e.g.- "en")
        /// </summary>
        [XmlAttribute()]
        public string LanguageId { get; set; }

        /// <summary>
        /// Language name (e.g.- "English")
        /// </summary>
        [XmlAttribute()]
        public string LanguageName { get; set; }
    }


    /// <summary>
    /// Represents the value for TranslationType attribute in tagged HTML files.
    /// </summary>
    public enum TranslationType
    {
        /// <summary>
        /// This is machine-translated content that members of the community can edit.  We encourage you to improve the translation by clicking the Edit link associated with any sentence below.
        /// </summary>
        MT_Editable,
        /// <summary>
        /// This is machine-translated content.
        /// </summary>
        MT_NonEditable,
        /// <summary>
        /// This is machine-translated content that is provided for the Beta release. 
        /// </summary>
        MT_Betacontent,
        /// <summary>
        /// This is machine-translated content that is provided for the Beta release. A fully translated version of this content will be provided in a future release.
        /// </summary>
        MT_BetaRecycledContents,
        /// <summary>
        /// 
        /// </summary>
        MT_QualityEditable,
        /// <summary>
        /// 
        /// </summary>
        MT_QualityNonEditable
    }


    /// <summary>
    /// Provides methods for translation and to get the list of supported languages.
    /// </summary>
    public class TranslationManager
    {
        const short RETRY_ATTEMPTS = 3;
        const short SLEEP_SECONDS = 120;
        private Queue<DateTime> requestsTime = new Queue<DateTime>(90);
        TranslationServiceFacade mtservice = new TranslationServiceFacade();



        /// <summary>
        /// Reads the text from the specified source SNT file, translates the text and writes them to the specified translated SNT file.
        /// </summary>
        /// <param name="sourceSnt">The complete path of the SNT file to be translated.</param>
        /// <param name="translatedSnt">The complete path of the SNT file to which translated text is to be written.</param>
        /// <param name="sourceLanguage">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguage">A string representing the language code to translate the text into.</param>
        public void TranslateSnt(string sourceSnt, string translatedSnt, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrEmpty(sourceSnt))
                throw new ArgumentNullException("SourceSnt");
            if (string.IsNullOrEmpty(translatedSnt))
                throw new ArgumentNullException("TranslatedSnt");
            if (string.IsNullOrEmpty(sourceLanguage))
                throw new ArgumentNullException("SourceLanguage");
            if (string.IsNullOrEmpty(targetLanguage))
                throw new ArgumentNullException("TargetLanguage");
            if (!File.Exists(sourceSnt))
                throw new FileNotFoundException(string.Format("Could not find the source snt file {0}", sourceSnt));

            Logger.Log(LogType.Status, string.Format("[Starting] Translating SNT file: {0}", sourceSnt));
            using (SntChunkReader chunkReader = new SntChunkReader(sourceSnt))
            {
                using (SntWriter sntWriter = new SntWriter(translatedSnt))
                {
                    foreach (string[] chunk in chunkReader.ChunkCollection)
                    {
                        if (chunk.Length == 0)
                            continue;
                        foreach (string str in TranslateChunk(chunk, sourceLanguage, targetLanguage))
                        {
                            sntWriter.Write(str);
                        }
                    }
                }
            }
            Logger.Log(LogType.Status, string.Format("[Finished] translating SNT file: {0}", sourceSnt));
        }

        /// <summary>
        /// Translates the specified chunk of text and returns the translated chunk.
        /// </summary>
        /// <param name="chunk">An array of string representing the text to be translated</param>
        /// <param name="sourceLanguage">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguage">A string representing the language code to translate the text into.</param>
        /// <returns>An array of translated text.</returns>
        public string[] TranslateChunk(string[] chunk, string sourceLanguage, string targetLanguage)
        {

            if (string.IsNullOrEmpty(sourceLanguage))
                throw new ArgumentNullException("sourceLanguage");
            if (string.IsNullOrEmpty(targetLanguage))
                throw new ArgumentNullException("targetLanguage");

            requestsTime.Enqueue(DateTime.Now);

            if (requestsTime.Count >= 90)
            {
                int seconds = (int)DateTime.Now.Subtract(requestsTime.Dequeue()).TotalSeconds;
                if (seconds < 60)
                {
                    int sleepTime = 60 - seconds;
                    Logger.Log(LogType.Warning, "Too many translation requests sent in last 60 seconds! Waiting for " + sleepTime + " seconds before it continues...");
                    Thread.Sleep(sleepTime * 1000);
                }
            }

            string[] translatedChunk = null;
            short attempt = 0;
            while (true)
            {
                try
                {
                    Logger.Log(LogType.Status, "Translating a chunk of sentences | Count=" + chunk.Length);
                    translatedChunk = Translate(chunk, sourceLanguage, targetLanguage);
                    break;
                }
                catch (Exception ex)
                {
                    if (attempt < RETRY_ATTEMPTS)
                    {
                        Logger.Log(LogType.Error, string.Format("Error while translating this chunk of sentences. Retrying in {0} seconds", SLEEP_SECONDS), ex);
                        attempt++;
                        Thread.Sleep(SLEEP_SECONDS * 1000); //convert to milliseconds
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (translatedChunk == null)
            {
                Logger.Log(LogType.Status, string.Format("All the {0} attempts failed to translate this chunk of sentences. Returning the source sentences back.", RETRY_ATTEMPTS));
                Logger.Log(LogType.UntranslatedSnt, "-----------------------------------------------------------------------------");
                foreach (string str in chunk)
                    Logger.Log(LogType.UntranslatedSnt, str);
                Logger.Log(LogType.UntranslatedSnt, "-----------------------------------------------------------------------------");
                translatedChunk = chunk;
            }
            return translatedChunk;
        }

        private string[] Translate(string[] chunk, string sourceLanguage, string targetLanguage)
        {
            //List<int> untranslatedLines;
            string[] translatedChunk = TranslationServiceFacade.TranslateArray(chunk, sourceLanguage, targetLanguage);     //mtService.Translate(sourceLanguage, targetLanguage, chunk, out untranslatedLines);
            /*
            foreach (int line in untranslatedLines)
            {
                Logger.Log(LogType.UntranslatedSnt, string.Format("[From: {0} - To: {1}] {2}", sourceLanguage, targetLanguage, chunk[line]));
            }
            */
            return translatedChunk;
        }

        /// <summary>
        /// Translates the specified list of string and returns the translated list of string.
        /// </summary>
        /// <param name="inputStrings">List of strings to be translated</param>
        /// <param name="sourceLanguage">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguage">A string representing the language code to translate the text into.</param>
        /// <returns>List of translated strings</returns>
        public List<string> TranslateTerms(List<string> inputStrings, string sourceLanguage, string targetLanguage)
        {
            List<string> translatedStrings = new List<string>();
            string[] chunk;
            List<string> tempChunk = new List<string>();
            int prevSize = 0;

            foreach (string line in inputStrings)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                string currLine = line;
                Byte[] bytesData = encoding.GetBytes(currLine);
                prevSize = prevSize + bytesData.Length;

                if (prevSize <= Statics.CHUNK_SIZE)
                {
                    tempChunk.Add(currLine);
                    currLine = String.Empty;
                }
                else
                {
                    chunk = tempChunk.ToArray();

                    translatedStrings.AddRange(TranslateChunk(chunk, sourceLanguage, targetLanguage));
                    prevSize = 0;
                    tempChunk.Clear();
                    tempChunk.Add(currLine);
                }
            }
            if (tempChunk.Count > 0)
            {
                chunk = tempChunk.ToArray();
                translatedStrings.AddRange(TranslateChunk(chunk, sourceLanguage, targetLanguage));
            }

            return translatedStrings;
        }
    }
}
