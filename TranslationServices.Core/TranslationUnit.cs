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
        private string _errortext;

        public string strSource {
            get { return this._strSource;}
            set {this._strSource = value;}
        }
        public string strTarget {
            get { return this._strTarget;}
            set {this._strTarget = value;}
        }
        public string comment {
            get { return this._comment;}
            set {this._comment = value;}
        }
        public int rating {
            get { return this._rating;}
            set {this._rating = value;}
        }
        public string user {
            get { return this._user;}
            set {this._user = value;}
        }
        public TUStatus status
        {
            get { return this._status; }
            set { this._status = value; }
        }
        public string errortext
        {
            get { return this._errortext; }
            set { this._errortext = value; }
        }


        public TranslationUnit()
        {

        }
        public TranslationUnit(string strSource, string strTarget, int rating, string user, string comment, TUStatus status, string errortext)
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
