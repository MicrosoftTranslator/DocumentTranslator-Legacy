using System;
using System.Collections;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// The HtmlNode is the base for all objects that may appear in HTML. Currently, 
    /// this implemention only supports HtmlText and HtmlElement node types.
    /// </summary>
    public abstract class HtmlNode
    {
        protected HtmlElement _parent;

        /// <summary>
        /// This will render the node as it would appear in HTML.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        /// This will return the parent of this node, or null if there is none.
        /// </summary>
        /// <returns>Parent of current node </returns>
        public HtmlElement Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// This will return the next sibling node. If this is the last one, it will return null.
        /// </summary>
        /// <returns>Next node of current node</returns>
        public HtmlNode Next
        {
            get
            {
                if (Index == -1)
                {
                    return null;
                }
                else
                {
                    if (Parent.Nodes.Count > Index + 1)
                    {
                        return Parent.Nodes[Index + 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// This will return the previous sibling node. If this is the first one, it will return null.
        /// </summary>
        /// <returns>Previous node of current node</returns>
        public HtmlNode Previous
        {
            get
            {
                if (Index == -1)
                {
                    return null;
                }
                else
                {
                    if (Index > 0)
                    {
                        return Parent.Nodes[Index - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// This will return the first child node. If there are no children, this
        /// will return null.
        /// </summary>
        /// <returns>First child of current node</returns>
        public HtmlNode FirstChild
        {
            get
            {
                if (this is HtmlElement)
                {
                    if (((HtmlElement)this).Nodes.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return ((HtmlElement)this).Nodes[0];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This will return the last child node. If there are no children, this
        /// will return null.
        /// </summary>
        /// <returns>Last child of current node</returns>
        public HtmlNode LastChild
        {
            get
            {
                if (this is HtmlElement)
                {
                    if (((HtmlElement)this).Nodes.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return ((HtmlElement)this).Nodes[((HtmlElement)this).Nodes.Count - 1];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This will return the index position within the parent's nodes that this one resides.
        /// If this is not in a collection, this will return -1.
        /// </summary>
        /// <returns>Index of the node</returns>
        public int Index
        {
            get
            {
                if (_parent == null)
                {
                    return -1;
                }
                else
                {
                    return _parent.Nodes.IndexOf(this);
                }
            }
        }

        /// <summary>
        /// This will return true if this is a root node (has no parent).
        /// </summary>
        /// <returns>Value indicating whether current node is root node</returns>
        public bool IsRoot
        {
            get
            {
                return _parent == null;
            }
        }

        /// <summary>
        /// This will return true if this is a child node (has a parent).
        /// </summary>
        /// <returns>Value indicating whether current node is child node or not</returns>
        public bool IsChild
        {
            get
            {
                return _parent != null;
            }
        }

        /// <summary>
        ///   This will return true if this is a parent node else false.
        /// </summary>
        /// <returns>Value indicating whether current node is parent node or not</returns>
        public bool IsParent
        {
            get
            {
                if (this is HtmlElement)
                {
                    return ((HtmlElement)this).Nodes.Count > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This will return true if the node passed is a descendent of this node.
        /// </summary>
        /// <param name="node">The node that might be the parent or grandparent (etc.)</param>
        /// <returns>True if this node is a descendent of the one passed in.</returns>
        public bool IsDescendentOf(HtmlNode node)
        {
            HtmlNode parent = _parent;
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
        public bool IsAncestorOf(HtmlNode node)
        {
            return node.IsDescendentOf(this);
        }

        /// <summary>
        /// This will return the ancstor that is common to this node and the one specified.
        /// </summary>
        /// <param name="node">The possible node that is relative</param>
        /// <returns>The common ancestor, or null if there is none</returns>
        public HtmlNode GetCommonAncestor(HtmlNode node)
        {
            HtmlNode thisParent = this;
            while (thisParent != null)
            {
                HtmlNode thatParent = node;
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

        private bool _addLineBreaks = false;
        public bool AddLineBreaks
        {
            get
            {
                return _addLineBreaks;
            }
            set
            {
                _addLineBreaks = value;
            }
        }

        /// <summary>
        /// This will remove this node and all child nodes from the tree. If this
        /// is a root node, this operation will do nothing.
        /// </summary>
        public void Remove()
        {
            if (_parent != null)
            {
                _parent.Nodes.RemoveAt(this.Index);
            }
        }

        /// <summary>
        /// This will return the full HTML to represent this node (and all child nodes).
        /// </summary>
        /// <returns>return HTML</returns>
        public abstract string HTML { get; }

        /// <summary>
        /// This will return true  current ndoe is text node else returns false.
        /// </summary>
        /// <returns>Value indicating whether current node is text node or not.</returns>
        public bool IsText()
        {
            return this is HtmlText;
        }

        /// <summary>
        /// This will return true  current ndoe is Element node else returns false.
        /// </summary>
        /// <returns>Value indicating whether current node is Element node or not.</returns>
        public bool IsElement()
        {
            return this is HtmlElement;
        }

        /// <summary>
        /// This constructor is used by the subclasses.
        /// </summary>
        protected HtmlNode()
        {
            _parent = null;
        }

        /// <summary>
        /// Internal method to maintain the identity of the parent node.
        /// </summary>
        /// <param name="parentNode">The parent node of this one</param>
        internal void SetParent(HtmlElement parentNode)
        {
            _parent = parentNode;
        }

        //### XHTML ###
        ///// <summary>
        ///// This will return the full XHTML to represent this node (and all child nodes)
        ///// </summary>
        //public abstract string XHTML { get; }
    }


}
