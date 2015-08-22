using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    /// <summary>
    /// This class is intended to hold a Translation Meory (TM) containing TranslationUnits as in the original TMX or XLIFF, only one source and target element, and only one language
    /// </summary>
    public class TranslationMemory
    {
        private List<TranslationUnit> _TranslationUnits;
        private string _sourceLangID;
        private string _targetLangID;

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

        }

        public TranslationMemory(List<TranslationUnit> TranslationUnits, string sourceLangID, string targetLangID)
        {
            this._sourceLangID = sourceLangID;
            this._targetLangID = targetLangID;
            this._TranslationUnits = TranslationUnits;
        }

        #region Public Methods

        public void AddTU(TranslationUnit TU)
        {
            this._TranslationUnits.Add(TU);
        }


        #endregion

    }
}
