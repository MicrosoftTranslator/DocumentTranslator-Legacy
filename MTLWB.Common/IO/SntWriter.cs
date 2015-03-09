using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using MTLWB.HtmlParser;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Represents a writer that can write a sequential series of lines to a SNT file.
    /// </summary>
    public class SntWriter : IDisposable 
    {

        FileStream SntFileStream = null;
        StreamWriter SntStreamWriter = null;
        string[] SentenceBreaker = new string[] { ". ", "! ", "; ", "? " };

        /// <summary>
        /// Number of lines written to SNT file so far.
        /// </summary>
        public int LineCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The complete path of SNT file to be written
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by SntReader.
        /// </summary>
        public void Dispose()
        {
            SntStreamWriter.Close();
            SntFileStream.Close();
            SntStreamWriter.Dispose();
            SntFileStream.Dispose();
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SntWriter class for the specified SNT file.
        /// </summary>
        /// <param name="fileName">The complete file path of SNT file to be written</param>
        public SntWriter(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.FileName = fileName;
            SntFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            SntStreamWriter = new StreamWriter(SntFileStream, Encoding.Unicode);
        }

        /// <summary>
        /// Writes the specified text to the next line in SNT file.
        /// </summary>
        /// <param name="text">Text to be written at the next line in the SNT file.</param>
        /// <returns>Line number at which the text is written.</returns>
        public int Write(string text)
        {
            SntStreamWriter.WriteLine(text);
            LineCount++;
            return LineCount;
        }

        /// <summary>
        /// Writes the specified text to the SNT file.
        /// </summary>
        /// <param name="text">Text to be written to the SNT file</param>
        /// <param name="breakLines">Value indicating whether to break specified text into multiple lines using list of sentence breakers.</param>
        /// <returns>string representing the comma separated SNT line numbers at which the text is written</returns>
        public string Write(string text, bool breakLines)
        {
            List<string> breakedLines = new List<string>();
            if (breakLines)
                breakedLines = BreakLine(text);
            else
                breakedLines.Add(text);

            string[] lineNumbers = new string[breakedLines.Count];

                for (int i = 0; i < breakedLines.Count; i++)
                {
                        SntStreamWriter.WriteLine(breakedLines[i].ToString());
                        LineCount++;

                    lineNumbers[i] = LineCount.ToString();
                }
            return string.Join(",", lineNumbers);
        }

        /// <summary>
        /// Writes the specified chunk (array of string) to the SNT file.
        /// </summary>
        /// <param name="chunk">An array of string to be written to the SNT file.</param>
        public void WriteChunk(string[] chunk)
        {
            foreach (string str in chunk)
            {
                Write(str);
            }
        }

        /// <summary>
        /// Breaks the data into multiple lines on the basis of sentence breaker defined.
        /// </summary>
        /// <param name="lineToBreak">Data to be broken</param>
        /// <returns>Breaked lines</returns>
        private List<string> BreakLine(string lineToBreak)
        {
            List<string> breakedLines = new List<string>();
            int startIndex = 0;
            int delimiterIndex = 0;
            do
            {
                //delimiterIndex = lineToBreak.IndexOfAny(_sentenceBreaker, startIndex);
                delimiterIndex = GetLineBreakIndex(lineToBreak, startIndex);

                if (delimiterIndex > -1)
                {
                    string breakLine = lineToBreak.Substring(startIndex, (delimiterIndex + 1 - startIndex));
                    breakedLines.Add(breakLine);
                    startIndex = delimiterIndex + 1;
                }
                else
                {
                    string breakLine = lineToBreak.Substring(startIndex, (lineToBreak.Length - startIndex));
                    if (breakLine.Trim() != string.Empty)
                    {
                        breakedLines.Add(breakLine);
                    }
                }
            } while (delimiterIndex > -1);

            return breakedLines;
        }

        private int GetLineBreakIndex(string lineToBreak, int startIndex)
        {
            int delimiterIndex = lineToBreak.Length - 1;
            int newIndex = -1;
            foreach (string breaker in SentenceBreaker)
            {
                newIndex = lineToBreak.IndexOf(breaker, startIndex);
                if (newIndex > -1)
                {
                    if (delimiterIndex > newIndex)
                    {
                        delimiterIndex = newIndex;
                    }
                }
            }
            //No line break string is found
            if (delimiterIndex == lineToBreak.Length - 1)
            {
                delimiterIndex = -1;
            }
            return delimiterIndex;
        }

    }
}
