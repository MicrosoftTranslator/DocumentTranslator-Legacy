// -
// <copyright file="TmxDocument.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System.Text;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// This is the basic HTML document object used to represent a sequence of HTML.
    /// </summary>
    public class TmxDocument
    {
        private TmxNodeCollection nodes;

        /// <summary>
        /// Initializes a new instance of the TmxDocument class.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        internal TmxDocument(string html, bool wantSpaces, bool addLineBreaks)
        {
            this.nodes = new TmxNodeCollection(null);

            TmxParser parser = new TmxParser();
            parser.RemoveEmptyElementText = !wantSpaces;
            this.nodes = parser.Parse(html, addLineBreaks);
        }

        /// <summary>
        /// Gets the collection of nodes used to represent this document.
        /// </summary>
        public TmxNodeCollection Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        /// <summary>
        /// Gets HTML used to represent this document.
        /// </summary>
        /// <returns>HTML of current object</returns>
        public string HTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                foreach (TmxNode node in this.Nodes)
                {
                    html.Append(node.HTML);
                }

                return html.ToString();
            }
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <returns>An instance of the newly created object.</returns>
        public static TmxDocument Create(string html)
        {
            return new TmxDocument(html, false, false);
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">Set this to true if you want to preserve all whitespace from the input HTML.</param>
        /// <returns>An instance of the newly created object.</returns>
        public static TmxDocument Create(string html, bool wantSpaces)
        {
            return new TmxDocument(html, wantSpaces, false);
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">Set this to true if you want to preserve all whitespace from the input HTML.</param>
        /// <returns>An instance of the newly created object.</returns>
        public static TmxDocument Create(string html, bool wantSpaces, bool addLineBreaks)
        {
            return new TmxDocument(html, wantSpaces, addLineBreaks);
        }
    }
}
