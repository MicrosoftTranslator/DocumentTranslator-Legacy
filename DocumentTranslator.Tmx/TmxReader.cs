// -
// <copyright file="TmxReader.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mts.Common.Tmx.Parser;

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Represents a reader that can read a sequential series of tags from a TMX file.
    /// </summary>
    public class TmxReader : IDisposable
    {
        private bool endOfFile;

        private StreamReader streamReader;

        private string tempStr;

        private TmxTag currentTmxTag;
        
        /// <summary>
        /// Initializes a new instance of the TmxReader class for the specified TMX file.
        /// </summary>
        /// <param name="fileName">The complete path of the TMX file to be read.</param>
        public TmxReader(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("Could not find the TMX file: '{0}'", fileName));
            }

            this.endOfFile = false;
            this.tempStr = string.Empty;
            this.streamReader = new StreamReader(fileName);
        }

        /// <summary>
        /// Initializes a new instance of the TmxReader class for the specified TMX file.
        /// </summary>
        /// <param name="stream">Specifies an input stream.</param>
        public TmxReader(StreamReader stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.streamReader = stream;
        }

        /// <summary>
        /// Gets a collection of TMX tags.
        /// </summary>
        public IEnumerable<TmxTag> TmxTags
        {
            get
            {
                while (!this.endOfFile)
                {
                    yield return this.GetNextTag();
                }

                yield break;
            }
        }

        /// <summary>
        /// Releases all the resources used by TmxReader.
        /// </summary>
        public void Dispose()
        {
            this.streamReader.Close();
            this.streamReader.Dispose();
        }

        /// <summary>
        /// Reads and returns the next tag in the TMX file.
        /// </summary>
        /// <returns>The next tag in the TMX file.</returns>
        private TmxTag GetNextTag()
        {
            string line = null;
            StringBuilder sb = new StringBuilder();
            string lineupper = String.Empty;
            
            int startIndex = -1;
            int endIndex = -1;
            TmxTagType tmxTagType = TmxTagType.NONE;

            this.currentTmxTag.TmxTagType = TmxTagType.NONE;
            this.currentTmxTag.Value = String.Empty;

            while ((line = this.streamReader.ReadLine()) != null || this.tempStr.Length > 0)
            {
                startIndex = -1;
                endIndex = -1;
                line = this.tempStr + line;
                lineupper = line.ToUpperInvariant();

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.XMLOPEN))
                {
                    startIndex = lineupper.IndexOf(Token.XMLOPEN);
                    tmxTagType = TmxTagType.XML;
                }

                if (endIndex == -1 && lineupper.Contains(Token.XMLCLOSE))
                {
                    endIndex = lineupper.IndexOf(Token.XMLCLOSE) + 2;
                }

                if (startIndex == -1 && 
                    endIndex == -1 && 
                    lineupper.Contains(Token.DOCTYPETAG))
                {
                    startIndex = lineupper.IndexOf(Token.DOCTYPETAG);
                    tmxTagType = TmxTagType.DOCTYPE;
                }

                if (endIndex == -1 && 
                    lineupper.Contains(Token.GT) && 
                    tmxTagType == TmxTagType.DOCTYPE)
                {
                    endIndex = lineupper.IndexOf(Token.GT) + 1;
                }

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.TMXOPEN))
                {
                    startIndex = lineupper.IndexOf(Token.TMXOPEN);
                    tmxTagType = TmxTagType.TMX_OPEN;
                }

                if (endIndex == -1 && 
                    lineupper.Contains(Token.GT) && 
                    tmxTagType == TmxTagType.TMX_OPEN)
                {
                    endIndex = lineupper.IndexOf(Token.GT) + 1;
                }

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.HEADEROPEN))
                {
                    startIndex = lineupper.IndexOf(Token.HEADEROPEN);
                    tmxTagType = TmxTagType.HEADER;
                }

                if (endIndex == -1 &&
                    lineupper.Contains(Token.HEADERCLOSE) && 
                    tmxTagType == TmxTagType.HEADER)
                {
                    endIndex = lineupper.IndexOf(Token.HEADERCLOSE) + 9;
                }

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.BODYOPEN))
                {
                    startIndex = lineupper.IndexOf(Token.BODYOPEN);
                    tmxTagType = TmxTagType.BODY_OPEN;
                }

                if (endIndex == -1 && 
                    lineupper.Contains(Token.GT) && 
                    tmxTagType == TmxTagType.BODY_OPEN)
                {
                    endIndex = lineupper.IndexOf(Token.GT) + 1;
                }

                if (startIndex == -1 &&
                    endIndex == -1 &&
                    (lineupper.Contains(Token.TUOPEN) ||
                     lineupper.Contains(Token.TUOPEN1) ||
                     lineupper.Trim().EndsWith(Token.TUOPEN2)))
                {
                    startIndex = lineupper.IndexOf(Token.TUOPEN2);
                    tmxTagType = TmxTagType.TU;
                }

                if (endIndex == -1 &&
                    lineupper.Contains(Token.TUCLOSE) && tmxTagType == TmxTagType.TU)
                {
                    endIndex = lineupper.IndexOf(Token.TUCLOSE) + 5;
                }

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.BODYCLOSE))
                {
                    startIndex = lineupper.IndexOf(Token.BODYCLOSE);
                    endIndex = startIndex + 7;
                    tmxTagType = TmxTagType.BODY_CLOSE;
                }

                if (startIndex == -1 && 
                    endIndex == -1 &&
                    lineupper.Contains(Token.TMXCLOSE))
                {
                    startIndex = lineupper.IndexOf(Token.TMXCLOSE);
                    endIndex = startIndex + 6;
                    tmxTagType = TmxTagType.TMX_CLOSE;
                }

                if (startIndex == -1 && endIndex == -1)
                {
                    sb.AppendFormat(" {0}", line);
                }
                else if (startIndex != -1 && endIndex == -1)
                {
                    sb.Clear();
                    sb.Append(line.Substring(startIndex));
                }
                else if (startIndex != -1 && endIndex != -1)
                {
                    sb.Clear();
                    sb.Append(line.Substring(startIndex, endIndex - startIndex));
                }
                else if (startIndex == -1 && endIndex != -1)
                {
                    sb.AppendFormat(" {0}", line.Substring(0, endIndex));
                }

                if (endIndex != -1)
                {
                    this.tempStr = line.Substring(endIndex);
                    this.currentTmxTag.TmxTagType = tmxTagType;
                    this.currentTmxTag.Value = sb.ToString();

                    if (this.streamReader.EndOfStream &&
                        this.tempStr.Trim().Length == 0)
                    {
                        this.endOfFile = true;
                    }

                    break;
                }
                else
                {
                    this.tempStr = string.Empty;
                }
            }
            
            if (this.streamReader.EndOfStream && 
                this.tempStr.Trim().Length == 0)
            {
                this.endOfFile = true;
            }

            return this.currentTmxTag;
        }
    }
}
