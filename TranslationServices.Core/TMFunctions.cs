using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    class TMFunctions
    {
        struct AddTranslationSegmentsResponse { public int segmentsprocessed; public int sentencesadded; public int errorsegments;};

        private static AddTranslationSegmentsResponse AddTranslationSegments(
            TranslationAssistant.TranslationServices.Core.TranslationMemory TM,
            TranslationAssistant.TranslationServices.Core.TranslationMemory TMErrors,
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
                Task<int[]> BSTaskFrom = TranslationAssistant.TranslationServices.Core.TranslationServiceFacade.BreakSentencesAsync(segment.strSource, fromlanguage);
                Task<int[]> BSTaskTo = TranslationAssistant.TranslationServices.Core.TranslationServiceFacade.BreakSentencesAsync(segment.strTarget, tolanguage);
                int[] fromoffsets = await BSTaskFrom;
                int[] tooffsets = await BSTaskTo;
                if (fromoffsets.Length != tooffsets.Length)
                {
                    TranslationAssistant.TranslationServices.Core.TranslationUnit ErrorSegment = new TranslationAssistant.TranslationServices.Core.TranslationUnit();
                    ErrorSegment = segment;
                    ErrorSegment.errortext = "Error 100: Different number of sentences in segment. Not added.";
                    ErrorSegment.status = TranslationAssistant.TranslationServices.Core.TUStatus.countmismatch;
                    TMErrors.Add(ErrorSegment);
                }
                else
                {
                    int fromstartindex = 0;
                    int tostartindex = 0;
                    for (int i = 0; i < fromoffsets.Length; i++)
                    {
                        TranslationAssistant.TranslationServices.Core.TranslationServiceFacade.AddTranslation(
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
