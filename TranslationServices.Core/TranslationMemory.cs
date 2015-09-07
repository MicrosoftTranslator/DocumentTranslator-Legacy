using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    /// <summary>
    /// This class is intended to hold a Translation Memory (TM) containing TranslationUnits as in the original TMX or XLIFF, only one source and target element, and only one language
    /// </summary>
    public class TranslationMemory : IEnumerable<TranslationUnit>
    {
        private List<TranslationUnit> _TranslationUnits = new List<TranslationUnit>();
        private string _sourceLangID = string.Empty;
        private string _targetLangID = string.Empty;

        public List<TranslationUnit> TranslationUnits
        {
            get { return _TranslationUnits; }
            set { _TranslationUnits = value; }
        }
        
        public string sourceLangID
        {
            get {return _sourceLangID;}
            set { _sourceLangID = value; }
        }

        public string targetLangID
        {
            get { return _targetLangID; }
            set { _targetLangID = value; }
        }


        public TranslationMemory()
        {
            List<TranslationUnit> _TranslationUnits = new List<TranslationUnit>();
        }

        #region Public Methods

        public void Add(TranslationUnit TU)
        {
            this._TranslationUnits.Add(TU);
        }

        public IEnumerator<TranslationUnit> GetEnumerator()
        {
            foreach (TranslationUnit TU in _TranslationUnits)
            {
                yield return TU;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

    }
}
