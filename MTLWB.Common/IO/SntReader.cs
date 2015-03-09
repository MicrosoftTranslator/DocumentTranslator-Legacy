using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Represents a reader that can read a sequential series of lines from a SNT file.
    /// </summary>
    public class SntReader : IDisposable
    {

        FileStream SntFileStream;
        StreamReader LineReader;
        int currentPosition = 0;
        string SntFile;

        /// <summary>
        /// Gets a value that indicates whether the current position of reader is at the end of the SNT file stream.
        /// </summary>
        public bool EOF
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the SntReader class for the specified SNT file.
        /// </summary>
        /// <param name="sntFile">The complete file path of the SNT file to be read.</param>
        public SntReader(string sntFile)
        {
            this.SntFile = sntFile;
            if (!File.Exists(sntFile))
            {
                throw new FileNotFoundException(sntFile + " file not found.");
            }
            Initialize();
        }

        private void Initialize()
        {
            SntFileStream = new FileStream(SntFile, FileMode.Open, FileAccess.Read);
            LineReader = new StreamReader(SntFileStream, Encoding.Unicode);
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by SntReader.
        /// </summary>
        public void Dispose()
        {
            LineReader.Close();
            SntFileStream.Close();
            LineReader.Dispose();
            SntFileStream.Dispose();
        }

        #endregion

        /// <summary>
        /// Reads the line in SNT file at specified line number.
        /// </summary>
        /// <param name="lineNumber">Line Number of the line to be Read from SNT file</param>
        /// <returns>The text in the SNT file at the specified line number.</returns>
        public string ReadLine(int lineNumber)
        {
            string Line = String.Empty;
            if (lineNumber < currentPosition)
            {
                Dispose();
                Initialize();
            }
            while (!LineReader.EndOfStream)
            {
                string currentLine = LineReader.ReadLine();
                currentPosition++;
                if (lineNumber == currentPosition)
                {
                    Line = currentLine;
                    break;
                }
            }
            EOF = LineReader.EndOfStream;
            return Line;
        }

        /// <summary>
        /// Reads the text of the next line in the SNT file.
        /// </summary>
        /// <returns>Text at next line in the SNT file.</returns>
        public string ReadLine()
        {
            if (LineReader.EndOfStream)
                throw new IOException("End of File has reached.");

            string line = LineReader.ReadLine();
            if (LineReader.EndOfStream)
                EOF = true;
            return line;
        }

        /// <summary>
        /// Reads all lines from the current position to the end of the SNT file and returns them as an array of string.
        /// </summary>
        /// <returns>An array of string containing all the lines from the current position to the end of the SNT file.</returns>
        public string[] ReadToEnd()
        {
            List<string> lines = new List<string>();
            while (!LineReader.EndOfStream)
            {
                lines.Add(LineReader.ReadLine());
            }
            EOF = true;
            return lines.ToArray();
        }
    }
}
