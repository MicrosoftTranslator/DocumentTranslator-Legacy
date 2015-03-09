using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

namespace MTLWB.HxS
{
    internal class Statics
    {
        // -----------------------------------------------------------------------------------------------
        // AVOID DEADLOCKS - When acquiring both locks at once you MUST always acquire in this order: 
        // 
        //     PreviousDenialOfServiceDictionary
        //     DenialOfServiceDictionary
        //
        // Better yet design such that you never need both at once.
        // -----------------------------------------------------------------------------------------------

        public static Uri URI { get { return s_URI; } }
        public static string AppPath { get { return s_strAppPath; } }
        public static int RequestSoftCharLimit { get { return s_iReqSoftCharLimit; } }
        public static int RequestHardCharLimit { get { return s_iReqHardCharLimit; } }
        public static int RequestArraySizeLimit { get { return s_iReqArraySizeLimit; } }
        public static bool IntraSententialTag(string strTagName) { return s_dctIntraSententialTags.ContainsKey(strTagName); }
        public static string TranslationSystemsMarkup { get { return s_strTranslationSystemsMarkup; } }
        public static int PageCacheTimeOut { get { return s_iPageCacheTimeOut; } }
        public static int PageCacheMaxSize { get { return s_iPageCacheMaxSize; } }

        /// <summary>
        /// This dictionary is keyed by the IP address of the client.  The value is an integer which contains
        /// the useage counter over the last 10 seconds for that IP address.  This covers the current
        /// 10 second interval.
        /// </summary>
        public static Dictionary<long, int> DenialOfServiceDictionary
        {
            get { return s_denialOfServiceDictionary; }
            set { s_denialOfServiceDictionary = value; }
        }

        /// <summary>
        /// This dictionary is keyed by the IP address of the client.  The value is an integer which contains
        /// the useage counter over the last 10 seconds for that IP address.  This covers the previous 10
        /// second interval.
        /// </summary>
        public static Dictionary<long, int> PreviousDenialOfServiceDictionary
        {
            get { return s_previousDenialOfServiceDictionary; }
            set { s_previousDenialOfServiceDictionary = value; }
        }

        /// <summary>
        /// If a particular IP address exceeds this threshold in a 10 second window, then it enters DOS mode.
        /// </summary>
        public static int DenialOfServiceThreshold { get { return s_denialOfServiceThreshold; } }

        static string s_strAppPath;
        static Uri s_URI;
        static int s_iReqSoftCharLimit, s_iReqHardCharLimit, s_iReqArraySizeLimit;
        static Dictionary<string, bool> s_dctIntraSententialTags;
        static string s_strTranslationSystemsMarkup;
        static int s_iPageCacheTimeOut, s_iPageCacheMaxSize;

        static int s_denialOfServiceThreshold;
        static Dictionary<long, int> s_denialOfServiceDictionary;
        static Dictionary<long, int> s_previousDenialOfServiceDictionary;
        static Timer s_janitorTimer;

        public static void Init(string strIniPath, string strAppPath, Uri uri)
        {
            s_strAppPath = strAppPath;
            s_URI = uri;
            s_denialOfServiceDictionary = new Dictionary<long, int>();
            s_previousDenialOfServiceDictionary = new Dictionary<long, int>();

            // Load config settings.
            using (StreamReader sr = new StreamReader(strAppPath+"\\"+strIniPath))
            {
                for (string str = sr.ReadLine(); str != null; str = sr.ReadLine())
                {
                    if (str.Contains("="))
                    {
                        switch (str.Substring(0, str.IndexOf("=")).Trim())
                        {
                            case "ReqSoftByteLimit":    // Devide by two because C# strings are UTF-16 (2 bytes / char)
                                s_iReqSoftCharLimit = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim()) / 2;
                                break;

                            case "ReqHardByteLimit":    // Devide by two because C# strings are UTF-16 (2 bytes / char)
                                s_iReqHardCharLimit = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim()) / 2;
                                break;

                            case "ReqArrayLimit":
                                s_iReqArraySizeLimit = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim());
                                break;

                            case "IntraSententialTags":
                                s_dctIntraSententialTags = new Dictionary<string, bool>();
                                foreach (string strTag in str.Substring(str.IndexOf("=") + 1).Trim().Split(new char[] { ',' }))
                                    //s_dctIntraSententialTags[strTag.Trim().ToLower()] = true;
                                    s_dctIntraSententialTags[strTag.Trim()] = true;
                                break;

                            case "PageCacheTimeOut":
                                s_iPageCacheTimeOut = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim());
                                break;

                            case "PageCacheMaxSize":
                                s_iPageCacheMaxSize = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim());
                                break;

                            case "DenialOfServiceThreshold":
                                s_denialOfServiceThreshold = int.Parse(str.Substring(str.IndexOf("=") + 1).Trim());
                                break;
                        }
                    }
                }
            }
            /*
                        // Grab the translation system list from the MT server.
                        mtSoapServiceSoapClient mtss = new mtSoapServiceSoapClient();
                        //mtss.Credentials = CredentialCache.DefaultNetworkCredentials;
                        List<TranslationSystem> liTS = new List<TranslationSystem>(
                            (mtss.GetTranslationSystems(new TranslationSystemRequest())).vTranslationSystems);

                        liTS.Sort(new Comparison<TranslationSystem>(TranslationSystemComparison));

                        // Instead of keeping around the list of translation systems lets just cache the markup string.
                        StringBuilder sb = new StringBuilder();
                        foreach (TranslationSystem ts in liTS)
                        {
                            sb.Append(string.Format("<option value=\"{0}_{1}\" onclick=\"SetLangPair(this.value);\">{2} - {3}</option>",
                                ts.structLangPair.strSourceLanguage,
                                ts.structLangPair.strTargetLanguage,
                                NormLangDesc(ts.structLangPair.strSourceLanguage),
                                NormLangDesc(ts.structLangPair.strTargetLanguage)));
                        }
             */
            //s_strTranslationSystemsMarkup = sb.ToString();
            s_strTranslationSystemsMarkup = "";

            // Initialize the denial of service timer thread
            TimerCallback callbackDelegate = new TimerCallback(Janitor.Wakeup);
            int delayUntilStart = 10 * 1000;  // 10 sec
            int refireDelay = 10 * 1000;
            s_janitorTimer = new Timer(callbackDelegate, null, delayUntilStart, refireDelay);
        }

        /*
                static int TranslationSystemComparison(TranslationSystem ts1, TranslationSystem ts2)
                {
                    if (NormLangDesc(ts1.structLangPair.strSourceLanguage) != NormLangDesc(ts2.structLangPair.strSourceLanguage))
                        return NormLangDesc(ts1.structLangPair.strSourceLanguage).CompareTo(NormLangDesc(ts2.structLangPair.strSourceLanguage));
                    else
                        return NormLangDesc(ts1.structLangPair.strTargetLanguage).CompareTo(NormLangDesc(ts2.structLangPair.strTargetLanguage));
                }

                static string NormLangDesc(string strLangName)
                {
                    CultureInfo ci = new CultureInfo(strLangName);
                    string strLangDesc = ci.DisplayName.Split(new char[] { ' ' })[0] + " (" + ci.Name.Split(new char[] { '-' })[1] + ")";
                    return strLangDesc;
                }
         */
    }

}