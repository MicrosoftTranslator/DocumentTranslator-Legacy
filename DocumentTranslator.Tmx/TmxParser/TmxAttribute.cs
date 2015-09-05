// -
// <copyright file="TmxAttribute.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections;
using System.ComponentModel;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// The TmxAttribute object represents a named value associated with an HtmlElement.
    /// </summary>
    public class TmxAttribute
    {
        private string name;
        private string value;

        /// <summary>
        /// Initializes a new instance of the TmxAttribute class.
        /// </summary>
        public TmxAttribute()
        {
            this.name = "Unnamed";
            this.value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the TmxAttribute class.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public TmxAttribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets name of the attribute. e.g. WIDTH.
        /// </summary>
        /// <returns>Value of _name.</returns>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets value of the attribute. e.g. 100%.
        /// </summary>
        /// <returns>Value of _value.</returns>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets HTML of this attribute.
        /// </summary>
        /// <returns>Value of _name.</returns>
        public string HTML
        {
            get
            {
                if (this.value == null)
                {
                    return this.name;
                }
                else
                {
                    return this.name + Token.EQDQ + this.value + Token.DQ;
                }
            }
        }

        /// <summary>
        /// This will return an HTML-formatted version of this attribute. 
        /// </summary>
        /// <returns>Value of _name.</returns>
        public override string ToString()
        {
            if (this.value == null)
            {
                return this.name;
            }
            else
            {
                return this.name + Token.EQDQ + this.value + Token.DQ;
            }
        }
    }
}
