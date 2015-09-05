// -
// <copyright file="TmxElement.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Text;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// The TmxElement object represents any HTML element. An element has a name
    /// and zero or more attributes.
    /// </summary>
    public class TmxElement : TmxNode
    {
        private string name;
        private TmxNodeCollection nodes;
        private TmxAttributeCollection attributes;
        private bool isTerminated;
        private bool isExplicitlyTerminated;

        /// <summary>
        /// Initializes a new instance of the TmxElement class.
        /// </summary>
        /// <param name="name">The name of this element.</param>
        public TmxElement(string name)
        {
            this.nodes = new TmxNodeCollection(this);
            this.attributes = new TmxAttributeCollection(this);
            this.name = name;
            this.isTerminated = false;
        }

        /// <summary>
        /// Gets or sets tag name of the element. e.g. BR, BODY, TABLE etc.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets collection of all child nodes of this one. If this node is actually
        /// a text node, this will throw an InvalidOperationException exception.
        /// </summary>
        public TmxNodeCollection Nodes
        {
            get
            {
                if (this.IsText())
                {
                    throw new InvalidOperationException("An HtmlText node does not have child nodes");
                }

                return this.nodes;
            }
        }

        /// <summary>
        /// Gets collection of attributes associated with this element.
        /// </summary>
        public TmxAttributeCollection Attributes
        {
            get
            {
                return this.attributes;
            }
        }
           
        /// <summary>
        /// Gets text of the element.
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (TmxNode node in this.Nodes)
                {
                    if (node is TmxText)
                    {
                        stringBuilder.Append(((TmxText)node).Text);
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
                html.Append("<");
                html.Append(this.name);

                foreach (TmxAttribute attribute in this.Attributes)
                {
                    html.Append(Token.SPACE);
                    html.Append(attribute.HTML);
                }

                if (this.Nodes.Count > 0)
                {
                    html.Append(Token.GT);

                    if (AddLineBreaks)
                    {
                        html.Append(Token.CRNL);
                    }

                    foreach (TmxNode node in this.Nodes)
                    {
                        html.Append(node.HTML);
                    }

                    html.Append(Token.LTSL);
                    html.Append(this.name);
                    html.Append(Token.GT);

                    if (AddLineBreaks)
                    {
                        html.Append(Token.CRNL);
                    }
                }
                else
                {
                    if (this.IsExplicitlyTerminated)
                    {
                        html.Append(Token.GT);
                        html.Append(Token.LTSL);
                        html.Append(this.name);
                        html.Append(Token.GT);

                        if (AddLineBreaks)
                        {
                            html.Append(Token.CRNL);
                        }
                    }
                    else if (this.IsTerminated)
                    {
                        html.Append(Token.SLGT);

                        if (AddLineBreaks)
                        {
                            html.Append(Token.CRNL);
                        }
                    }
                    else
                    {
                        html.Append(Token.GT);

                        if (AddLineBreaks)
                        {
                            html.Append(Token.CRNL);
                        }
                    }
                }

                return html.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is explicitly closed using the 'name' method.
        /// </summary>
        internal bool IsTerminated
        {
            get
            {
                if (this.Nodes.Count > 0)
                {
                    return false;
                }
                else
                {
                    return this.isTerminated | this.isExplicitlyTerminated;
                }
            }

            set
            {
                this.isTerminated = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is explicitly closed using the 'name' method.
        /// </summary>
        internal bool IsExplicitlyTerminated
        {
            get
            {
                return this.isExplicitlyTerminated;
            }

            set
            {
                this.isExplicitlyTerminated = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is "script" or "style" in the name.
        /// </summary>
        internal bool NoEscaping
        {
            get
            {
                return Str.IsEqual(Token.SCRIPT, this.Name, true) || Str.IsEqual(Token.STYLE, this.Name, true);
            }
        }

        /// <summary>
        /// This will return the HTML representation of this element.
        /// </summary>
        /// <returns>Return a valid string.</returns>
        public override string ToString()
        {
            string value = Token.LT + this.name;

            foreach (TmxAttribute attribute in this.Attributes)
            {
                value += Token.SPACE + attribute.ToString();
            }

            value += Token.GT;
            return value;
        }
    }
}
