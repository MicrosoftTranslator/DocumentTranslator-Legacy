// -
// <copyright file="SntWriterContainer.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Container class to store all language sentence stream writer object.
    /// </summary>
    public class SntWriterContainer : IDisposable
    {
        private string fileName;

        private string extension;

        private Dictionary<string, SntWriter> sntWriters;

        private bool released;

        /// <summary>
        /// Initializes a new instance of the SntWriterContainer class.
        /// </summary>
        /// <param name="fileName">Specifies a base file name.</param>
        /// <param name="extension">Specifies a output file extension.</param>
        public SntWriterContainer(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
                        
            this.fileName = fileName;

            if (string.IsNullOrEmpty(extension))
            {
                this.extension = ".snt";
            }
            else
            {
                this.extension = extension;
            }

            this.sntWriters = new Dictionary<string, SntWriter>();
        }

        /// <summary>
        /// Gets all languages output tmx file paths.
        /// </summary>
        public string[] Files
        {
            get
            {
                if (this.sntWriters.Count == 0)
                {
                    return null;
                }

                string[] files = new string[this.sntWriters.Count];

                int i = 0;

                foreach (KeyValuePair<string, SntWriter> item in this.sntWriters)
                {
                    Debug.Assert(item.Value != null, "dictionary item is null");
                    files[i++] = item.Value.FilePath;
                }

                return files;
            }
        }

        /// <summary>
        /// Gets the Indexer to retrieve a sentence stream writer for a langauge.
        /// </summary>
        /// <param name="language">Specifies a language.</param>
        /// <returns>Return sentence stream writer.</returns>
        public SntWriter this[string language]
        {
            get
            {
                return this.GetWriter(language);
            }
        }

        /// <summary>
        /// Closes all sentence stream writers.
        /// </summary>
        public void Close()
        {
            if (!this.released)
            {
                foreach (KeyValuePair<string, SntWriter> item in this.sntWriters)
                {
                    Debug.Assert(item.Value != null, "dictionary item is null");
                    item.Value.Dispose();
                }

                this.released = true;
            }
        }

        /// <summary>
        /// Disposes this object by closing all sentence stream writers.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a sentence stream writer object for the language.  
        /// </summary>
        /// <param name="language">Specifies a language.</param>
        /// <returns>Returns a valid sentence writer object.</returns>
        private SntWriter GetWriter(string language)
        {
            language = language.ToLower();

            Debug.Assert(!string.IsNullOrEmpty(language), "language is empty");
            SntWriter writer = null;

            if (this.sntWriters.ContainsKey(language))
            {
                writer = this.sntWriters[language];
            }
            else
            {
                Debug.Assert(!string.IsNullOrEmpty(this.fileName), "base file name is empty");
                Debug.Assert(this.sntWriters != null, "Snt writer container is null");

                string filePath = string.Format("{0}_{1}{2}", this.fileName, language, this.extension);

                writer = new SntWriter(filePath);
                this.sntWriters.Add(language.ToLower(), writer);
            }

            Debug.Assert(writer != null, "writer object is null");
            return writer;
        }
    }
}
