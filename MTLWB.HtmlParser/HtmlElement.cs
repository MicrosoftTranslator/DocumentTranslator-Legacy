using System;
using System.Text;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// The HtmlElement object represents any HTML element. An element has a name
    /// and zero or more attributes.
    /// </summary>
    public class HtmlElement : HtmlNode
    {
        protected string _name;
        protected HtmlNodeCollection _nodes;
        protected HtmlAttributeCollection _attributes;
        protected bool _isTerminated;
        protected bool _isExplicitlyTerminated;

        /// <summary>
        /// This constructs a new HTML element with the specified tag name.
        /// </summary>
        /// <param name="name">The name of this element</param>
        public HtmlElement(string name)
        {
            _nodes = new HtmlNodeCollection(this);
            _attributes = new HtmlAttributeCollection(this);
            _name = name;
            _isTerminated = false;
        }

        /// <summary>
        /// This is the tag name of the element. e.g. BR, BODY, TABLE etc.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// This is the collection of all child nodes of this one. If this node is actually
        /// a text node, this will throw an InvalidOperationException exception.
        /// </summary>
        public HtmlNodeCollection Nodes
        {
            get
            {
                if (IsText())
                {
                    throw new InvalidOperationException("An HtmlText node does not have child nodes");
                }
                return _nodes;
            }
        }

        /// <summary>
        /// This is the collection of attributes associated with this element.
        /// </summary>
        public HtmlAttributeCollection Attributes
        {
            get
            {
                return _attributes;
            }
        }
     
        /// <summary>
        /// This will return the HTML representation of this element.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string value = "<" + _name;
            foreach (HtmlAttribute attribute in Attributes)
            {
                value += " " + attribute.ToString();
            }
            value += ">";
            return value;
        }

        /// <summary>
        /// This function creates test of the element
        /// </summary>
        /// <returns>Text</returns>
        public string Text
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (HtmlNode node in Nodes)
                {
                    if (node is HtmlText)
                    {
                        stringBuilder.Append(((HtmlText)node).Text);
                    }
                }
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// This will return the HTML for this element and all subnodes.
        /// </summary>
        public override string HTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                html.Append("<" + _name);
                foreach (HtmlAttribute attribute in Attributes)
                {
                    html.Append(" " + attribute.HTML);
                }
                if (Nodes.Count > 0)
                {
                    html.Append(">");
                    if (AddLineBreaks)
                        html.Append("\r\n");
                    foreach (HtmlNode node in Nodes)
                    {
                        html.Append(node.HTML);
                    }
                    html.Append("</" + _name + ">");
                    if (AddLineBreaks)
                        html.Append("\r\n");
                }
                else
                {
                    if (IsExplicitlyTerminated)
                    {
                        html.Append("></" + _name + ">");
                        if (AddLineBreaks)
                            html.Append("\r\n");
                    }
                    else if (IsTerminated)
                    {
                        html.Append("/>");
                        if (AddLineBreaks)
                            html.Append("\r\n");
                    }
                    else
                    {
                        html.Append(">");
                        if (AddLineBreaks)
                            html.Append("\r\n");
                    }
                }
                return html.ToString();
            }

        }

        /// <summary>
        /// This flag indicates that the element is explicitly closed using the "<name/>" method.
        /// </summary>
        internal bool IsTerminated
        {
            get
            {
                if (Nodes.Count > 0)
                {
                    return false;
                }
                else
                {
                    return _isTerminated | _isExplicitlyTerminated;
                }
            }
            set
            {
                _isTerminated = value;
            }
        }

        /// <summary>
        /// This flag indicates that the element is explicitly closed using the "</name>" method.
        /// </summary>
        internal bool IsExplicitlyTerminated
        {
            get
            {
                return _isExplicitlyTerminated;
            }
            set
            {
                _isExplicitlyTerminated = value;
            }
        }

        /// <summary>
        /// This returns value indiacting whether there is "script" or "style" in the name.
        /// </summary>
        internal bool NoEscaping
        {
            get
            {
                return "script".Equals(Name.ToLower()) || "style".Equals(Name.ToLower());
            }
        }

        //### XHTML ###
        ///// <summary>
        ///// This will return the XHTML for this element and all subnodes.
        ///// </summary>
        //public override string XHTML
        //{
        //    get
        //    {
        //        if ("html".Equals(_name) && this.Attributes["xmlns"] == null)
        //        {
        //            Attributes.Add(new HtmlAttribute("xmlns", "http://www.w3.org/1999/xhtml"));
        //        }
        //        StringBuilder html = new StringBuilder();
        //        html.Append("<" + _name.ToLower());
        //        foreach (HtmlAttribute attribute in Attributes)
        //        {
        //            html.Append(" " + attribute.XHTML);
        //        }
        //        if (IsTerminated)
        //        {
        //            html.Append("/>");
        //        }
        //        else
        //        {
        //            if (Nodes.Count > 0)
        //            {
        //                html.Append(">");
        //                foreach (HtmlNode node in Nodes)
        //                {
        //                    html.Append(node.XHTML);
        //                }
        //                html.Append("</" + _name.ToLower() + ">");
        //            }
        //            else
        //            {
        //                html.Append("/>");
        //            }
        //        }
        //        return html.ToString();
        //    }
        //}
    }
}
