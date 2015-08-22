// -
// <copyright file="TmxText.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.ComponentModel;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// The TmxText node represents a simple piece of text from the document.
    /// </summary>
    public class TmxText : TmxNode
    {
        private string text;

        /// <summary>
        /// Initializes a new instance of the TmxText class.
        /// </summary>
        /// <param name="text">Specifies a html text.</param>
        public TmxText(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Gets or sets the text associated with this node.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
            }
        }
        
        /// <summary>
        /// This will return the HTML to represent this text object.
        /// </summary>
        public override string HTML
        {
            get
            {
                if (this.NoEscaping)
                {
                    return this.Text;
                }
                else
                {
                    return this.Text;
                }
            }
        }

        internal bool NoEscaping
        {
            get
            {
                if (this.Parent == null)
                {
                    return false;
                }
                else
                {
                    return ((TmxElement)this.Parent).NoEscaping;
                }
            }
        }

        /// <summary>
        /// This will return the text for outputting inside an HTML document.
        /// </summary>
        /// <returns>Return a string.</returns>
        public override string ToString()
        {
            return this.Text;
        }
    }
}
