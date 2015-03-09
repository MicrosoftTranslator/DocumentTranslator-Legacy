using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTLWB.MTService.TranslationService;
using TranslationAssistant.DocumenTranslationAssistant;

namespace MTLWB.MTService
{
    internal class PublicTranslator : Translator
    {
        LanguageServiceClient soapClient;

        public PublicTranslator()
        {
            try
            {
                soapClient = new LanguageServiceClient("BasicHttpBinding_LanguageService");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PublicTranslator(string serviceUrl)
        {
            try
            {
                soapClient = new LanguageServiceClient("BasicHttpBinding_LanguageService", serviceUrl);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public override string[] GetLanguageCodes()
        {
            return soapClient.GetLanguagesForTranslate(AppId);
        }

        public override string[] GetLanguageNames(string[] langIds)
        {
            return soapClient.GetLanguageNames(AppId, "en", langIds);
        }
        public override string Translate(string sourceLanguageId, string targetLanguageId, string text)
        {
            return TranslationServiceFacade.
        }

        public override string[] Translate(string sourceLanguageId, string targetLanguageId, string[] text, out List<int> untranslatedLines)
        {
            
            string[] translatedText = new string[text.Length];
            untranslatedLines = new List<int>();
            TranslateOptions options = new TranslateOptions();
            if(!string.IsNullOrEmpty(Category))
                options.Category = Category;
            options.ContentType = "text/html";

            Dictionary<int, string> longSentences = new Dictionary<int, string>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Length > 2000)
                {
                    longSentences.Add(i, text[i]);
                    text[i] = "";
                }
            }

            try
            {
                TranslateArrayResponse[] response = soapClient.TranslateArray(AppId, text, sourceLanguageId, targetLanguageId, options);
                for (int i = 0; i < text.Length; i++)
                {
                    if (longSentences.ContainsKey(i))
                    {
                        translatedText[i] = longSentences[i];
                        untranslatedLines.Add(i);
                    }
                    else if (!string.IsNullOrEmpty(response[i].Error) || text[i] == response[i].TranslatedText)
                    {
                        translatedText[i] = text[i];
                        untranslatedLines.Add(i);
                    }
                    else
                    {
                        translatedText[i] = response[i].TranslatedText;
                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return translatedText;

        }

    }
}
