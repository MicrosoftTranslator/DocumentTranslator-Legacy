using System;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// This is the basic HTML document object used to represent a sequence of HTML.
    /// </summary>
    public class HtmlDocument
    {
        HtmlNodeCollection _nodes = new HtmlNodeCollection(null);

        //### XHTML ###
        //private string _xhtmlHeader = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        internal HtmlDocument(string html, bool wantSpaces, bool addLineBreaks)
        {
            HtmlParser parser = new HtmlParser();
            parser.RemoveEmptyElementText = !wantSpaces;
            _nodes = parser.Parse(html, addLineBreaks);
        }

        //### XHTML ###
        //public string DocTypeXHTML
        //{
        //    get
        //    {
        //        return _xhtmlHeader;
        //    }
        //    set
        //    {
        //        _xhtmlHeader = value;
        //    }
        //}

        /// <summary>
        /// This is the collection of nodes used to represent this document.
        /// </summary>
        public HtmlNodeCollection Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <returns>An instance of the newly created object.</returns>
        public static HtmlDocument Create(string html)
        {
            return new HtmlDocument(html, false, false);
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">Set this to true if you want to preserve all whitespace from the input HTML</param>
        /// <returns>An instance of the newly created object.</returns>
        public static HtmlDocument Create(string html, bool wantSpaces)
        {
            return new HtmlDocument(html, wantSpaces, false);
        }


        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">Set this to true if you want to preserve all whitespace from the input HTML</param>
        /// <returns>An instance of the newly created object.</returns>
        public static HtmlDocument Create(string html, bool wantSpaces, bool addLineBreaks)
        {
            return new HtmlDocument(html, wantSpaces, addLineBreaks);
        }


        /// <summary>
        /// This will return the HTML used to represent this document.
        /// </summary>
        /// <returns>HTML of current object</returns>
        public string HTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                foreach (HtmlNode node in Nodes)
                {
                    html.Append(node.HTML);
                }
                return html.ToString();
            }
        }

        //### XHTML ###
        ///// <summary>
        ///// This will return the XHTML document used to represent this document.
        ///// </summary>
        //public string XHTML
        //{
        //    get
        //    {
        //        StringBuilder html = new StringBuilder();
        //        if (_xhtmlHeader != null)
        //        {
        //            html.Append(_xhtmlHeader);
        //            html.Append("\r\n");
        //        }
        //        foreach (HtmlNode node in Nodes)
        //        {
        //            html.Append(node.XHTML);
        //        }
        //        return html.ToString();
        //    }
        //}
    }
}
