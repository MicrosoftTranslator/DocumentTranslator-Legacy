using System.IO;

namespace TranslationAssistant.Business
{
    public static partial class TranslationBusinessHelper
    {
        /// <summary>
        /// List of languages that do not use space as word saparator.
        /// Must be listed in all lowercase.
        /// </summary>
        public const string nonspacelanguages = "zh, th, ja, ko, zh-hans, zh-hant, zh-chs, zh-cht";


        public static string Splitevenly(string utterance, int segments, string langcode)
        {
            if (segments <= 1) return utterance;
            StringWriter result = new StringWriter();
            int segmentlength = utterance.Length / segments;
            if (nonspacelanguages.Contains(langcode.ToLowerInvariant()))    //non-spacing languages
            {
                for (int i = 0; i < segments; i++) result.WriteLine(utterance.Substring(segmentlength * i, segmentlength));
            }
            else                                                            //spacing languages
            {
                int startindex = 0;
                for (int i = 1; i < segments; i++)
                {
                    int endindex = utterance.IndexOf(' ', segmentlength * i);
                    if (endindex > startindex) result.WriteLine(utterance.Substring(startindex, endindex - startindex));
                    startindex = endindex + 1;
                }
                result.WriteLine(utterance.Substring(startindex));          //copy the last segment
            }
            string debug = result.ToString();
            return result.ToString();
        }
    }
}
