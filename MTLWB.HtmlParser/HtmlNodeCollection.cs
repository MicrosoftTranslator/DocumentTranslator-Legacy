using System;
using System.Collections;
using System.ComponentModel;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// This object represents a collection of HtmlNodes, which can be either HtmlText
    /// or HtmlElement objects. The order in which the nodes occur directly corresponds
    /// to the order in which they appear in the original HTML document.
    /// </summary>
    public class HtmlNodeCollection : CollectionBase
    {
        private HtmlElement _parent;
   
        /// <summary>
        // Public constructor to create an empty collection.
        /// </summary>
        public HtmlNodeCollection()
        {
            _parent = null;
        }

        /// <summary>
        /// A collection is usually associated with a parent node (an HtmlElement, actually)
        /// but you can pass null to implement an abstracted collection.
        /// </summary>
        /// <param name="parent">The parent element, or null if it is not appropriate</param>
        internal HtmlNodeCollection(HtmlElement parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// This will add a node to the collection.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int Add(HtmlNode node)
        {
            if (_parent != null) node.SetParent(_parent);
            return base.List.Add(node);
        }

        /// <summary>
        /// This is used to identify the index of this node as it appears in the collection.
        /// </summary>
        /// <param name="node">The node to test</param>
        /// <returns>The index of the node, or -1 if it is not in this collection</returns>
        public int IndexOf(HtmlNode node)
        {
            return base.List.IndexOf(node);
        }

        /// <summary>
        /// This will insert a node at the given position
        /// </summary>
        /// <param name="index">The position at which to insert the node.</param>
        /// <param name="node">The node to insert.</param>
        public void Insert(int index, HtmlNode node)
        {
            if (_parent != null) node.SetParent(_parent);
            base.InnerList.Insert(index, node);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the
        /// specified name. If you want to search the subnodes recursively, you should
        /// pass True as the parameter in searchChildren. This search is guaranteed to
        /// return nodes in the order in which they are found in the document.
        /// </summary>
        /// <param name="name">The name of the element to find</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByName(string name)
        {
            return FindByName(name, true);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the
        /// specified name. If you want to search the subnodes recursively, you should
        /// pass True as the parameter in searchChildren. This search is guaranteed to
        /// return nodes in the order in which they are found in the document.
        /// </summary>
        /// <param name="name">The name of the element to find</param>
        /// <param name="searchChildren">True if you want to search sub-nodes, False to
        /// only search this collection.</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByName(string name, bool searchChildren)
        {
            HtmlNodeCollection results = new HtmlNodeCollection(null);
            foreach (HtmlNode node in base.List)
            {
                if (node is HtmlElement)
                {
                    if (((HtmlElement)node).Name.ToLower().Equals(name.ToLower()))
                    {
                        results.Add(node);
                    }
                    if (searchChildren)
                    {
                        foreach (HtmlNode matchedChild in ((HtmlElement)node).Nodes.FindByName(name, searchChildren))
                        {
                            results.Add(matchedChild);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the an
        /// attribute with the given name. 
        /// </summary>
        /// <param name="name">The name of the attribute to find</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByAttributeName(string attributeName)
        {
            return FindByAttributeName(attributeName, true);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the an
        /// attribute with the given name. 
        /// </summary>
        /// <param name="name">The name of the attribute to find</param>
        /// <param name="searchChildren">True if you want to search sub-nodes, False to
        /// only search this collection.</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByAttributeName(string attributeName, bool searchChildren)
        {
            HtmlNodeCollection results = new HtmlNodeCollection(null);
            foreach (HtmlNode node in base.List)
            {
                if (node is HtmlElement)
                {
                    foreach (HtmlAttribute attribute in ((HtmlElement)node).Attributes)
                    {
                        if (attribute.Name.ToLower().Equals(attributeName.ToLower()))
                        {
                            results.Add(node);
                            break;
                        }
                    }
                    if (searchChildren)
                    {
                        foreach (HtmlNode matchedChild in ((HtmlElement)node).Nodes.FindByAttributeName(attributeName, searchChildren))
                        {
                            results.Add(matchedChild);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements with the an
        /// attribute with the given name and given value. 
        /// <param name="attributeName">Name of attribute to search</param>
        /// <param name="attributeValue">Value of attribute to search</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByAttributeNameValue(string attributeName, string attributeValue)
        {
            return FindByAttributeNameValue(attributeName, attributeValue, true);
        }

        /// <summary>
        /// This will search though this collection of nodes for all elements and child elements with the an
        /// attribute with the given name and given value. 
        /// <param name="attributeName">Name of attribute to search</param>
        /// <param name="attributeValue">Value of attribute to search</param>
        /// <param name="searchChildren">True if you want to search sub-nodes, False to
        /// only search this collection.</param>
        /// <returns>A collection of all the nodes that macth.</returns>
        public HtmlNodeCollection FindByAttributeNameValue(string attributeName, string attributeValue, bool searchChildren)
        {
            HtmlNodeCollection results = new HtmlNodeCollection(null);
            foreach (HtmlNode node in base.List)
            {
                if (node is HtmlElement)
                {
                    foreach (HtmlAttribute attribute in ((HtmlElement)node).Attributes)
                    {
                        if (attribute.Name.ToLower().Equals(attributeName.ToLower()))
                        {
                            if (attribute.Value.ToLower().Equals(attributeValue.ToLower()))
                            {
                                results.Add(node);
                            }
                            break;
                        }
                    }
                    if (searchChildren)
                    {
                        foreach (HtmlNode matchedChild in ((HtmlElement)node).Nodes.FindByAttributeNameValue(attributeName, attributeValue, searchChildren))
                        {
                            results.Add(matchedChild);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// This property allows you to change the node at a particular position in the
        /// collection.
        /// </summary>
        public HtmlNode this[int index]
        {
            get
            {
                return (HtmlNode)base.InnerList[index];
            }
            set
            {
                if (_parent != null) value.SetParent(_parent);
                base.InnerList[index] = value;
            }
        }

        /// <summary>
        /// This allows you to directly access the first element in this colleciton with the given name.
        /// If the node does not exist, this will return null.
        /// </summary>
        public HtmlNode this[string name]
        {
            get
            {
                HtmlNodeCollection results = FindByName(name, false);
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
    }
}
