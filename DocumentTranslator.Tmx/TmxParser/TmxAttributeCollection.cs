// -
// <copyright file="TmxAttributeCollection.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections;
using System.ComponentModel;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// This is a collection of attributes. Typically, this is associated with a particular
    /// element. This collection is searchable by both the index and the name of the attribute.
    /// </summary>
    public class TmxAttributeCollection : CollectionBase
    {
        private TmxElement element;

        /// <summary>
        /// Initializes a new instance of the TmxAttributeCollection class.
        /// </summary>
        public TmxAttributeCollection()
        {
            this.element = null;
        }

        /// <summary>
        /// Initializes a new instance of the TmxAttributeCollection class.
        /// </summary>
        /// <param name="element">Specifies an element.</param>
        internal TmxAttributeCollection(TmxElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// This provides direct access to an attribute in the collection by its index.
        /// </summary>
        /// <returns>Specifies a html attribute.</returns>
        public TmxAttribute this[int index]
        {
            get { return (TmxAttribute)this.List[index]; }
            set { this.List[index] = value; }
        }
        
        /// <summary>
        /// This overload allows you to have direct access to an attribute by providing
        /// its name. If the attribute does not exist, null is returned.
        /// </summary>
        public TmxAttribute this[string name]
        {
            get
            {
                return this.FindByName(name);
            }
        }

        /// <summary>
        /// This will add an element to the collection.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        /// <returns>The index at which it was added.</returns>
        public int Add(TmxAttribute attribute)
        {
            return this.List.Add(attribute);
        }

        /// <summary>
        /// This will search the collection for the named attribute. If it is not found, this
        /// will return null.
        /// </summary>
        /// <param name="name">The name of the attribute to find.</param>
        /// <returns>The attribute, or null if it wasn't found.</returns>
        public TmxAttribute FindByName(string name)
        {
            int index = this.IndexOf(name);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this[this.IndexOf(name)];
            }
        }

        /// <summary>
        /// Added by snehal.
        /// This will search the collection for the named attribute. If it is not found, this
        /// will return null.
        /// </summary>
        /// <param name="value">The value of the attribute to find.</param>
        /// <returns>The attribute, or null if it wasn't found.</returns>
        public TmxAttribute FindByValue(string value)
        {
            int index = this.AIndexOf(value);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this[this.AIndexOf(value)];
            }
        }

        /// <summary>
        /// This will return the index of the attribute with the specified name. If it is
        /// not found, this method will return -1.
        /// </summary>
        /// <param name="name">The name of the attribute to find.</param>
        /// <returns>The zero-based index, or -1.</returns>
        public int IndexOf(string name)
        {
            for (int index = 0; index < this.List.Count; index++)
            {
                if (Str.IsEqual(this[index].Name, name, true))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// This will return the index of the attribute with the specified value. If it is
        /// not found, this method will return -1.
        /// </summary>
        /// <param name="value">The name of the attribute to find.</param>
        /// <returns>The zero-based index, or -1.</returns>
        public int AIndexOf(string value)
        {
            for (int index = 0; index < this.List.Count; index++)
            {
                if (Str.IsEqual(this[index].Value, value, true))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
