using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    public enum TUStatus { good, countmismatch, lengthmismatch, tagsmismatch };

    public class TranslationUnit
    {
        #region Fields and Enums

        
        private string _strSource;
        private string _strTarget;
        private string _comment;
        private int _rating;
        private string _user;
        private TUStatus _status;

        public TranslationUnit()
        {

        }
        public TranslationUnit(string strSource, string strTarget, int rating, string user, string comment, TUStatus status)
        {
            this._strSource = strSource;
            this._strTarget = strTarget;
            this._comment = comment;
            this._rating = rating;
            this._user = user;
            this._status = status;
        }
        #endregion
    }
}
