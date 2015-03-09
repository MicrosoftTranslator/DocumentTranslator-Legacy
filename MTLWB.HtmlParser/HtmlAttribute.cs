using System;
using System.Collections;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// The HtmlAttribute object represents a named value associated with an HtmlElement.
    /// </summary>
    public class HtmlAttribute
    {
        protected string _name;
        protected string _value;

        public HtmlAttribute()
        {
            _name = "Unnamed";
            _value = "";
        }

        /// <summary>
        /// This constructs an HtmlAttribute object with the given name and value.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public HtmlAttribute(string name, string value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// The name of the attribute. e.g. WIDTH
        /// </summary>
        /// <returns>value of _name</returns>
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
        /// The value of the attribute. e.g. 100%
        /// </summary>
        /// <returns>value of _value</returns>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// This will return an HTML-formatted version of this attribute. 
        /// </summary>
        /// <returns>value of _name</returns>
        public override string ToString()
        {
            if (_value == null)
            {
                return _name;
            }
            else
            {
                return _name + "=\"" + _value + "\"";
            }
        }

        /// <summary>
        /// This will return HTML of this attribute
        /// </summary>
        /// <returns>value of _name</returns>
        public string HTML
        {
            get
            {
                if (_value == null)
                {
                    return _name;
                }
                else
                {
                    //return _name + "=\"" + HtmlEncoder.EncodeValue(_value) + "\"";
                    return _name + "=\"" + _value + "\"";
                }
            }
        }

        //### XHTML ###
        //public string XHTML
        //{
        //    get
        //    {
        //        if (_value == null)
        //        {
        //            return _name.ToLower();
        //        }
        //        else
        //        {
        //            return _name + "=\"" + HtmlEncoder.EncodeValue(_value.ToLower()) + "\"";
        //        }
        //    }
        //}
    }
}
