using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTLWB.MTService.TranslationServiceLoc;

namespace MTLWB.MTService
{
    internal class LocTranslator: Translator
    {
        TranslationServiceContractClient soapClient;
        public LocTranslator()
        {
            try
            {
                soapClient = new TranslationServiceContractClient("BasicHttpBinding_ITranslationServiceContract1");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LocTranslator(string serviceUrl)
        {
            try
            {
                soapClient = new TranslationServiceContractClient("BasicHttpBinding_ITranslationServiceContract1", serviceUrl);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
        public override string[] GetLanguageCodes()
        {
            string[] langPairs = null;
            TranslationSystemRequest TSReq = new TranslationSystemRequest();
            TranslationSystemResponse TSResp = new TranslationSystemResponse();
            TSReq.DescriptionLanguage = "en-us";
            TSReq.AppID = AppId;

            TSResp = soapClient.GetTranslationSystems(TSReq);
            langPairs = new string[TSResp.TranslationSystems.Length];
            for(int i=0; i<TSResp.TranslationSystems.Length; i++)
            {
                langPairs[i] = TSResp.TranslationSystems[i].LangPair.SourceLanguage + "\t" + TSResp.TranslationSystems[i].LangPair.TargetLanguage;
            }

            return langPairs;
        }

        public override string[] GetLanguageNames(string[] langIds)
        {
            string[] langNames = new string[langIds.Length];
            TranslationSystemRequest TSReq = new TranslationSystemRequest();
            TranslationSystemResponse TSResp = new TranslationSystemResponse();
            TSReq.DescriptionLanguage = "en-us";
            TSReq.AppID = AppId;

            TSResp = soapClient.GetTranslationSystems(TSReq);
            
            for (int i = 0; i < langIds.Length; i++)
            {
                foreach (TSItem ts in TSResp.TranslationSystems)
                {
                    if (langIds[i] == ts.LangPair.SourceLanguage + "\t" + ts.LangPair.TargetLanguage)
                    {
                        langNames[i] = ts.Description;
                        break;
                    }
                }
            }

            return langNames;
            
        }

        public override string Translate(string sourceLanguageId, string targetLanguageId, string text)
        {
            throw new NotImplementedException();
        }

        public override string[] Translate(string sourceLanguageId, string targetLanguageId, string[] text, out List<int> untranslatedLines)
        {
            string[] translatedText = new string[text.Length];
            untranslatedLines = new List<int>();

            TranslationRequest tr = new TranslationRequest();
            tr.LangPair = new LanguagePair();
            tr.LangPair.SourceLanguage = sourceLanguageId;
            tr.LangPair.TargetLanguage = targetLanguageId;
            tr.Url = "*** MS Internal – MTLWB ***";
            tr.ProviderID = "msr-mt";
            tr.TransformID = "text/html";
            tr.AppID = AppId;
            tr.CustomizationIDs = new ArrayOfString1();
            tr.CustomizationIDs.Add(CustomizationId);
            tr.Texts = new ArrayOfString();
            tr.Texts.AddRange(text);

            TranslationResponse TR = soapClient.Translate(tr);
            if (TR.Translations != null)
            {
                for (int i = 0; i < TR.Translations.Length; i++)
                {
                    if (TR.Translations[i].Error != 0 || TR.Translations[i].Text.Length == 0 || TR.Translations[i].Text.Equals(text[i]))
                    {
                        translatedText[i] = text[i];
                        untranslatedLines.Add(i);
                    }
                    else
                    {
                        translatedText[i] = TR.Translations[i].Text;
                    }
                }
            }
            return translatedText;
        }
    }
}
