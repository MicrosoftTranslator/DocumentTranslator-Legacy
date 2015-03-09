using System;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// The HtmlText node represents a simple piece of text from the document.
    /// </summary>
    public class HtmlText : HtmlNode
    {
        protected string _text;

        /// <summary>
        /// This constructs a new node with the given text content.
        /// </summary>
        /// <param name="text"></param>
        public HtmlText(string text)
        {
            _text = text;
        }

        /// <summary>
        /// This is the text associated with this node.
        /// </summary>
        /// <return>
        /// Text String associated with node like if node is <HTML> then it returns html
        /// </return>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        
        /// <summary>
        /// This will return the HTML to represent this text object.
        /// </summary>
        public override string HTML
        {
            get
            {
                if (NoEscaping)
                {
                    return Text;
                }
                else
                {
                    //return HtmlEncoder.EncodeValue(Text);
                    return Text;
                }
            }
        }

        //### XHTML ###
        ///// <summary>
        ///// This will return the XHTML to represent this text object.
        ///// </summary>
        //public override string XHTML
        //{
        //    get
        //    {
        //        return HtmlEncoder.EncodeValue(Text);
        //    }
        //}

        /// <summary>
        /// This will return the text for outputting inside an HTML document.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        internal bool NoEscaping
        {
            get
            {
                if (_parent == null)
                {
                    return false;
                }
                else
                {
                    return ((HtmlElement)_parent).NoEscaping;
                }
            }
        }

    }
}
