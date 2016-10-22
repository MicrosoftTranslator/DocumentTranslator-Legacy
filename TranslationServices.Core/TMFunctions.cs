using System;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    public class TMFunctions
    {
        /// <summary>
        /// Return structure for the AddTranslationSegments method
        /// </summary>
        public struct AddTranslationSegmentsResponse {
            public int segmentsprocessed;
            public int sentencesadded;
            public int errorsegments;
            public int ratioviolations;
            public int tagviolations;
        };

        /// <summary>
        /// Break Sentences of arbitrary length segments and add to CTF
        /// </summary>
        /// <param name="TM">Input Translation Memory</param>
        /// <param name="TMErrors">Failed segments, with errors, in TM format</param>
        /// <param name="fromlanguage">From language code</param>
        /// <param name="tolanguage">To langauge code</param>
        /// <param name="Rating">Rating you want to apply to all added segments. Higher than 6 will override MT. Max is 10.</param>
        /// <param name="User">Arbitrary string representing the user. Only one entry per user allowed.</param>
        /// <param name="AddToCTF">If false, do not actually add, just perform error check</param>
        /// <returns></returns>
        public static AddTranslationSegmentsResponse AddTranslationSegments(
            TranslationMemory TM,
            TranslationMemory TMErrors,
            string fromlanguage,
            string tolanguage,
            int Rating,
            string User,
            bool AddToCTF
            )
        {
            AddTranslationSegmentsResponse response = new AddTranslationSegmentsResponse();
            response.sentencesadded = 0;
            response.ratioviolations = 0;
            response.tagviolations = 0;
            Parallel.ForEach(TM, async (segment) =>
            {

                //Decode the segments
                segment.strSource = System.Net.WebUtility.HtmlDecode(segment.strSource);
                segment.strTarget = System.Net.WebUtility.HtmlDecode(segment.strTarget);

                Task<int[]> BSTaskFrom = TranslationServiceFacade.BreakSentencesAsync(segment.strSource, fromlanguage);
                Task<int[]> BSTaskTo = TranslationServiceFacade.BreakSentencesAsync(segment.strTarget, tolanguage);

                //throw away segments with tags
                if ((segment.strSource.Contains("<") && segment.strSource.Contains(">")) && (segment.strTarget.Contains("<") && segment.strTarget.Contains(">")))
                {
                    response.tagviolations++;
                    TranslationUnit ErrorSegment = new TranslationUnit();
                    ErrorSegment = segment;
                    ErrorSegment.errortext = "Error 102: Segment contains tags. Not added.";
                    ErrorSegment.status = TUStatus.tagsmismatch;
                    TMErrors.Add(ErrorSegment);
                    return;
                }

                //throw away segments of a hugely differing length
                float ratio = Math.Abs(segment.strSource.Length / segment.strTarget.Length);
                if ((ratio > 3) && ((segment.strSource.Length > 15) || (segment.strTarget.Length > 15))) //skip the segment, and add to error.tmx
                {
                    response.ratioviolations++;
                    TranslationUnit ErrorSegment = new TranslationUnit();
                    ErrorSegment = segment;
                    ErrorSegment.errortext = "Error 101: Segment length ratio exceeded. Not added.";
                    ErrorSegment.status = TUStatus.lengthmismatch;
                    TMErrors.Add(ErrorSegment);
                    return;
                }

                //TODO: special handling of bpt/ept 



                int[] fromoffsets = await BSTaskFrom;
                int[] tooffsets = await BSTaskTo;

                if (fromoffsets.Length != tooffsets.Length)
                {
                    TranslationUnit ErrorSegment = new TranslationUnit();
                    ErrorSegment = segment;
                    ErrorSegment.errortext = "Error 100: Different number of sentences in segment. Not added.";
                    ErrorSegment.status = TUStatus.countmismatch;
                    TMErrors.Add(ErrorSegment);
                }
                else
                {
                    if (AddToCTF)
                    {
                        int fromstartindex = 0;
                        int tostartindex = 0;
                        for (int i = 0; i < fromoffsets.Length; i++)
                        {
                            TranslationServiceFacade.AddTranslation(
                                segment.strSource.Substring(fromstartindex, fromoffsets[i]),
                                segment.strTarget.Substring(tostartindex, tooffsets[i]),
                                fromlanguage,
                                tolanguage,
                                Rating,
                                User
                                );
                            fromstartindex = fromoffsets[i];
                            tostartindex = tooffsets[i];
                            response.sentencesadded++;
                        }
                    }
                }

            });
            response.errorsegments = TMErrors.Count;
            response.segmentsprocessed = TM.Count - TMErrors.Count;
            return response;
        }
    }
}
