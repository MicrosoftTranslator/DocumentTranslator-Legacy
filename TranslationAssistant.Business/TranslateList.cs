using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationAssistant.TranslationServices.Core;

namespace TranslationAssistant.Business
{
    public class TranslateList
    {
        public TranslateList()
        {

        }

        public async Task<List<string>> Translate(List<string> list, string from, string to)
        {
            List<string> translatedlist = new List<string>();
            List<Task<string>> tasklist = new List<Task<string>>();
            foreach (string text in list)
            {
                Task<string> task = TranslationServiceFacade.TranslateStringAsync(text, from, to);
                tasklist.Add(task);
            }
            string[] resultlist = await Task.WhenAll(tasklist);
            return resultlist.ToList<string>();
        }
    }
}
