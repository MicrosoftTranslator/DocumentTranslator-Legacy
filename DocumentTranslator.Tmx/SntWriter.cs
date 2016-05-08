// -
// <copyright file="SntWriter.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.IO;
using System.Text;
using System.Web;

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Represents a writer that can write a sequential series of lines to a SNT file.
    /// </summary>
    public class SntWriter : IDisposable 
    {
        private string[] sentenceBreaker = new string[] { ". ", "! ", "; ", "? " };

        private FileStream sntFileStream;

        private StreamWriter sntStreamWriter;

        /// <summary>
        /// Initializes a new instance of the SntWriter class for the specified SNT file.
        /// </summary>
        /// <param name="fileName">The complete file path of SNT file to be written.</param>
        public SntWriter(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            this.sntFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            this.sntStreamWriter = new StreamWriter(this.sntFileStream, Encoding.Unicode);
            this.FilePath = fileName;
        }

        /// <summary>
        /// Gets the number of lines written to SNT file so far.
        /// </summary>
        public int LineCount { get; private set; }

        /// <summary>
        /// Gets the file snt file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Releases all resources used by SntReader.
        /// </summary>
        public void Dispose()
        {
            this.sntStreamWriter.Close();
            this.sntFileStream.Close();
            this.sntStreamWriter.Dispose();
            this.sntFileStream.Dispose();
        }

        /// <summary>
        /// Writes the specified text to the next line in SNT file.
        /// </summary>
        /// <param name="text">Text to be written at the next line in the SNT file.</param>
        /// <returns>Line number at which the text is written.</returns>
        public int Write(string text)
        {
            this.sntStreamWriter.WriteLine(Unescape(text));
            this.LineCount++;
            return this.LineCount;
        }

        private string Unescape(string segment)
        {
            return HttpUtility.HtmlDecode(segment);
        }
    }
}
