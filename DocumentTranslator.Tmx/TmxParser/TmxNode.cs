// -
// <copyright file="TmxNode.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections;
using System.ComponentModel;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// The TmxNode is the base for all objects that may appear in HTML. Currently, 
    /// this implemention only supports HtmlText and HtmlElement node types.
    /// </summary>
    public abstract class TmxNode
    {
        private TmxElement parent;

        private bool addLineBreaks = false;

        /// <summary>
        /// Initializes a new instance of the TmxNode class.
        /// </summary>
        protected TmxNode()
        {
            this.parent = null;
        }

        /// <summary>
        /// Gets the parent of this node, or null if there is none.
        /// </summary>
        /// <returns>Parent of current node </returns>
        public TmxElement Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the next sibling node. If this is the last one, it will return null.
        /// </summary>
        /// <returns>Next node of current node</returns>
        public TmxNode Next
        {
            get
            {
                if (this.Index == -1)
                {
                    return null;
                }
                else
                {
                    if (this.Parent.Nodes.Count > this.Index + 1)
                    {
                        return this.Parent.Nodes[this.Index + 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous sibling node. If this is the first one, it will return null.
        /// </summary>
        /// <returns>Previous node of current node</returns>
        public TmxNode Previous
        {
            get
            {
                if (this.Index == -1)
                {
                    return null;
                }
                else
                {
                    if (this.Index > 0)
                    {
                        return this.Parent.Nodes[this.Index - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first child node. If there are no children, this will return null.
        /// </summary>
        public TmxNode FirstChild
        {
            get
            {
                if (this is TmxElement)
                {
                    if (((TmxElement)this).Nodes.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return ((TmxElement)this).Nodes[0];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the last child node. If there are no children, this will return null.
        /// </summary>
        public TmxNode LastChild
        {
            get
            {
                if (this is TmxElement)
                {
                    if (((TmxElement)this).Nodes.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return ((TmxElement)this).Nodes[((TmxElement)this).Nodes.Count - 1];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the index position within the parent's nodes that this one resides.
        /// If this is not in a collection, this will return -1.
        /// </summary>
        /// <returns>Index of the node</returns>
        public int Index
        {
            get
            {
                if (this.Parent == null)
                {
                    return -1;
                }
                else
                {
                    return this.Parent.Nodes.IndexOf(this);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a root node (has no parent).
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a child node (has a parent).
        /// </summary>
        public bool IsChild
        {
            get
            {
                return this.Parent != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a parent node.
        /// </summary>
        public bool IsParent
        {
            get
            {
                if (this is TmxElement)
                {
                    return ((TmxElement)this).Nodes.Count > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool AddLineBreaks
        {
            get { return this.addLineBreaks; }
            set { this.addLineBreaks = value; }
        }

        /// <summary>
        /// Gets the full HTML to represent this node (and all child nodes).
        /// </summary>
        /// <returns>return HTML</returns>
        public abstract string HTML { get; }

        /// <summary>
        /// This will return true if the node passed is a descendent of this node.
        /// </summary>
        /// <param name="node">The node that might be the parent or grandparent (etc).</param>
        /// <returns>True if this node is a descendent of the one passed in.</returns>
        public bool IsDescendentOf(TmxNode node)
        {
            TmxNode parent = this.Parent;
            while (parent != null)
            {
                if (parent == node)
                {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }

        /// <summary>
        /// This will return true if the node passed is one of the children or grandchildren of this node.
        /// </summary>
        /// <param name="node">The node that might be a child.</param>
        /// <returns>True if this node is an ancestor of the one specified.</returns>
        public bool IsAncestorOf(TmxNode node)
        {
            return node.IsDescendentOf(this);
        }

        /// <summary>
        /// This will return the ancstor that is common to this node and the one specified.
        /// </summary>
        /// <param name="node">The possible node that is relative.</param>
        /// <returns>The common ancestor, or null if there is none.</returns>
        public TmxNode GetCommonAncestor(TmxNode node)
        {
            TmxNode thisParent = this;
            while (thisParent != null)
            {
                TmxNode thatParent = node;
                while (thatParent != null)
                {
                    if (thisParent == thatParent)
                    {
                        return thisParent;
                    }

                    thatParent = thatParent.Parent;
                }

                thisParent = thisParent.Parent;
            }

            return null;
        }

        /// <summary>
        /// This will remove this node and all child nodes from the tree. If this
        /// is a root node, this operation will do nothing.
        /// </summary>
        public void Remove()
        {
            if (this.Parent != null)
            {
                this.Parent.Nodes.RemoveAt(this.Index);
            }
        }

        /// <summary>
        /// This will return true  current ndoe is text node else returns false.
        /// </summary>
        /// <returns>Value indicating whether current node is text node or not.</returns>
        public bool IsText()
        {
            return this is TmxText;
        }

        /// <summary>
        /// This will return true  current ndoe is Element node else returns false.
        /// </summary>
        /// <returns>Value indicating whether current node is Element node or not.</returns>
        public bool IsElement()
        {
            return this is TmxElement;
        }

        /// <summary>
        /// This will render the node as it would appear in HTML.
        /// </summary>
        /// <returns>Returns string value representing the node.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Internal method to maintain the identity of the parent node.
        /// </summary>
        /// <param name="parentNode">The parent node of this one.</param>
        internal void SetParent(TmxElement parentNode)
        {
            this.parent = parentNode;
        }
    }
}
