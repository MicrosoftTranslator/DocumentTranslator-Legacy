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
        private string _sourceLangID = string.Empty;
        private string _targetLangID = string.Empty;
        private List<TranslationUnit> translationUnits = new List<TranslationUnit>();

        public List<TranslationUnit> TranslationUnits { get => translationUnits; set => translationUnits = value; }
        public string sourceLangID
        {
            get { return _sourceLangID; }
            set { _sourceLangID = value; }
        }

        public string targetLangID
        {
            get { return _targetLangID; }
            set { _targetLangID = value; }
        }

        public int Count
        {
            get { return TranslationUnits.Count; }
        }


        public TranslationMemory()
        {
            List<TranslationUnit> _TranslationUnits = new List<TranslationUnit>();
        }

        #region Public Methods

        /// <summary>
        /// Write Translation memory into a TMX file of the name filename
        /// </summary>
        /// <param name="filename"></param>
        public void WriteToTmx(string filename)
        {
            using (Mts.Common.Tmx.TmxWriter Tmx = new Mts.Common.Tmx.TmxWriter(filename, sourceLangID, targetLangID, true))
            {
                foreach (TranslationUnit TU in this.TranslationUnits)
                {
                    Tmx.TmxWriteSegment(TU.strSource, TU.strTarget, sourceLangID, targetLangID, Mts.Common.Tmx.TmxWriter.TUError.good);
                }
            }
            return;
        }

        /// <summary>
        /// Add this Translation Unit TU to the Translation Memory
        /// </summary>
        /// <param name="TU"></param>
        public void Add(TranslationUnit TU)
        {
            this.TranslationUnits.Add(TU);
        }

        public IEnumerator<TranslationUnit> GetEnumerator()
        {
            foreach (TranslationUnit TU in TranslationUnits)
            {
                yield return TU;
            }
        }

        public static TranslationMemory Take(int fromindex, int toindex)
        {
            TranslationMemory TM = new TranslationMemory();

            //TODO: Take implementation goes here.
            return TM;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

    }
}
