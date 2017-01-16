using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    public class CustomLanguages
    {
        /// <summary>
        /// Holds a set of custom languages
        /// </summary>
        private StringDictionary _Languages;
        public int Count {
            get {
                if (_Languages == null) return 0;
                return _Languages.Count;
            }
        }

        /// <summary>
        /// Add a pair of [langcode, language friendly name] to the set of custom languages
        /// </summary>
        /// <param name="LangCode">Language code</param>
        /// <param name="LangName">Language friendly name</param>
        public void Add(string LangCode, string LangName)
        {
            _Languages.Add(LangCode, LangName);
        }

        /// <summary>
        /// Delete the entry for this language code
        /// </summary>
        /// <param name="LangCode">The code of the language to delete</param>
        public void Delete(string LangCode)
        {
            _Languages.Remove(LangCode);
        }

        public CustomLanguages()
        {
            _Languages = Properties.Settings.Default.CustomLanguages;
            if (_Languages == null) _Languages = new StringDictionary();
        }

        ~CustomLanguages()
        {
            Properties.Settings.Default.CustomLanguages = _Languages;
            Properties.Settings.Default.Save();
        }
    }
}
