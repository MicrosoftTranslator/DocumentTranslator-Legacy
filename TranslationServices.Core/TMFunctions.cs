using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    class TMFunctions
    {
        /// <summary>
        /// Return structure for the AddTranslationSegments method
        /// </summary>
        struct AddTranslationSegmentsResponse { public int segmentsprocessed; public int sentencesadded; public int errorsegments;};

        /// <summary>
        /// Break Sentences of arbitrary length segments and add to CTF
        /// </summary>
        /// <param name="TM">Input Translation Memory</param>
        /// <param name="TMErrors">Failed segments, with errors, in TM format</param>
        /// <param name="fromlanguage">From language code</param>
        /// <param name="tolanguage">To langauge code</param>
        /// <param name="Rating">Rating you want to apply to all added segments. Higher than 6 will override MT. Max is 10.</param>
        /// <param name="User">Arbitrary string representing the user. Only one entry per user allowed.</param>
        /// <returns></returns>
        private static AddTranslationSegmentsResponse AddTranslationSegments(
            TranslationMemory TM,
            TranslationMemory TMErrors,
            string fromlanguage,
            string tolanguage,
            int Rating,
            string User
            )
        {
            AddTranslationSegmentsResponse response = new AddTranslationSegmentsResponse();
            response.sentencesadded = 0;
            Parallel.ForEach(TM, async (segment) =>
            {
                Task<int[]> BSTaskFrom = TranslationServiceFacade.BreakSentencesAsync(segment.strSource, fromlanguage);
                Task<int[]> BSTaskTo = TranslationServiceFacade.BreakSentencesAsync(segment.strTarget, tolanguage);
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

            });
            response.errorsegments = TMErrors.Count;
            response.segmentsprocessed = TM.Count - TMErrors.Count;
            return response;
        }
    }
}
