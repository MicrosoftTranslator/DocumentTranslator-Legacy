// -
// <copyright file="TmxNodeCollection.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections;
using System.ComponentModel;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// This object represents a collection of HtmlNodes, which can be either HtmlText
    /// or HtmlElement objects. The order in which the nodes occur directly corresponds
    /// to the order in which they appear in the original HTML document.
    /// </summary>
    public class TmxNodeCollection : CollectionBase
    {
        private TmxElement parent;

        /// <summary>
        /// Initializes a new instance of the TmxNodeCollection class.
        /// </summary>
        public TmxNodeCollection()
        {
            this.parent = null;
        }

        /// <summary>
        /// Initializes a new instance of the TmxNodeCollection class.
        /// A collection is usually associated with a parent node (an HtmlElement, actually)
        /// but you can pass null to implement an abstracted collection.
        /// </summary>
        /// <param name="parent">The parent element, or null if it is not appropriate.</param>
        internal TmxNodeCollection(TmxElement parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// This property allows you to change the node at a particular position in the
        /// collection.
        /// </summary>
        public TmxNode this[int index]
        {
            get
            {
                return (TmxNode)this.InnerList[index];
            }

            set
            {
                if (this.parent != null)
                {
                    value.SetParent(this.parent);
                }

                this.InnerList[index] = value;
            }
        }

        /// <summary>
        /// This allows you to directly access the first element in this colleciton with the given name.
        /// If the node does not exist, this will return null.
        /// </summary>
        public TmxNode this[string name]
        {
            get
            {
                TmxNodeCollection results = this.FindByName(name, false);
                if (results.Count > 0)
                {
                    return results[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This will add a node to the collection.
        /// </summary>
        /// <param name="node">Specifies a node to be added.</param>
        /// <returns>Returns index of the newly added item.</returns>
        public int Add(TmxNode node)
        {
            if (this.parent != null)
            {
                node.SetParent(this.parent);
            }

            return this.List.Add(node);
        }

        /// <summary>
        /// This is used to identify the index of this node as it appears in the collection.
        /// </summary>
        /// <param name="node">The node to test.</param>
        /// <returns>The index of the node, or -1 if it is not in this collection.</returns>
        public int IndexOf(TmxNode node)
        {
            return this.List.IndexOf(node);
        }

        /// <summary>
        /// This will insert a node at the given position.
        /// </summary>
        /// <param name="index">The position at which to insert the node.</param>
        /// <param name="node">The node to insert.</param>
        public void Insert(int index, TmxNode node)
        {
            if (this.parent != null)
            {
                node.SetParent(this.parent);
            }

            this.InnerList.Insert(index, node);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the
        /// specified name. If you want to search the subnodes recursively, you should
        /// pass True as the parameter in searchChildren. This search is guaranteed to
        /// return nodes in the order in which they are found in the document.
        /// </summary>
        /// <param name="name">The name of the element to find.</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public TmxNodeCollection FindByName(string name)
        {
            return this.FindByName(name, true);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the
        /// specified name. If you want to search the subnodes recursively, you should
        /// pass True as the parameter in searchChildren. This search is guaranteed to
        /// return nodes in the order in which they are found in the document.
        /// </summary>
        /// <param name="name">The name of the element to find.</param>
        /// <param name="searchChildren">True if you want to search sub-nodes, False to
        /// only search this collection.</param>
        /// <returns>A collection of all the nodes that match.</returns>
        public TmxNodeCollection FindByName(string name, bool searchChildren)
        {
            TmxNodeCollection results = new TmxNodeCollection(null);

            foreach (TmxNode node in this.List)
            {
                if (node is TmxElement)
                {
                    if (((TmxElement)node).Name.ToLower().Equals(name.ToLower()))
                    {
                        results.Add(node);
                    }

                    if (searchChildren)
                    {
                        foreach (TmxNode matchedChild in ((TmxElement)node).Nodes.FindByName(name, searchChildren))
                        {
                            results.Add(matchedChild);
                        }
                    }
                }
            }

            return results;
        }
    }
}
