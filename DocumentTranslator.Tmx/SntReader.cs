// -
// <copyright file="SntReader.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
* This class contains valid code. Commented for code coverage purpose.
* 
namespace Mts.Common.Tmx
{
    /// <summary>
    /// Represents a reader that can read a sequential series of lines from a SNT file.
    /// </summary>
    public class SntReader : IDisposable
    {
        private FileStream sntFileStream;
        private StreamReader lineReader;
        private int currentPosition = 0;
        private string sntFilePath;

        /// <summary>
        /// Initializes a new instance of the SntReader class for the specified SNT file.
        /// </summary>
        /// <param name="sntFilePath">The complete file path of the SNT file to be read.</param>
        public SntReader(string sntFilePath)
        {
            if (string.IsNullOrEmpty(sntFilePath))
            {
                throw new ArgumentNullException("sntFile");
            }

            this.sntFilePath = sntFilePath;
            if (!File.Exists(sntFilePath))
            {
                throw new FileNotFoundException(sntFilePath + " file not found.");
            }

            this.Initialize();
        }

        /// <summary>
        /// Gets a value indicating whether the current position of reader is at the end of the SNT file stream.
        /// </summary>
        public bool Eof
        {
            get;
            private set;
        }

        /// <summary>
        /// Releases all resources used by SntReader.
        /// </summary>
        public void Dispose()
        {
            this.lineReader.Close();
            this.sntFileStream.Close();
            this.lineReader.Dispose();
            this.sntFileStream.Dispose();
        }

        /// <summary>
        /// Reads the line in SNT file at specified line number.
        /// </summary>
        /// <param name="lineNumber">Line Number of the line to be Read from SNT file.</param>
        /// <returns>The text in the SNT file at the specified line number.</returns>
        public string ReadLine(int lineNumber)
        {
            string line = String.Empty;
            if (lineNumber < this.currentPosition)
            {
                this.Dispose();
                this.Initialize();
            }

            while (!this.lineReader.EndOfStream)
            {
                string currentLine = this.lineReader.ReadLine();
                this.currentPosition++;

                if (lineNumber == this.currentPosition)
                {
                    line = currentLine;
                    break;
                }
            }

            this.Eof = this.lineReader.EndOfStream;
            return line;
        }

        /// <summary>
        /// Reads the text of the next line in the SNT file.
        /// </summary>
        /// <returns>Text at next line in the SNT file.</returns>
        public string ReadLine()
        {
            if (this.lineReader.EndOfStream)
            {
                throw new IOException("End of File has reached.");
            }

            string line = this.lineReader.ReadLine();
            if (this.lineReader.EndOfStream)
            {
                this.Eof = true;
            }

            return line;
        }

        /// <summary>
        /// Reads all lines from the current position to the end of the SNT file and returns them as an array of string.
        /// </summary>
        /// <returns>An array of string containing all the lines from the current position to the end of the SNT file.</returns>
        public string[] ReadToEnd()
        {
            List<string> lines = new List<string>();
            while (!this.lineReader.EndOfStream)
            {
                lines.Add(this.lineReader.ReadLine());
            }

            this.Eof = true;

            return lines.ToArray();
        }
        
        private void Initialize()
        {
            this.sntFileStream = new FileStream(this.sntFilePath, FileMode.Open, FileAccess.Read);
            this.lineReader = new StreamReader(this.sntFileStream, Encoding.Unicode);
        }
    }
}
*/