using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Enumerator defining the types of tags in TMX file.
    /// </summary>
    public enum TmxTagType
    {
        XML,
        DOCTYPE,
        TMX_OPEN,
        TMX_CLOSE,
        BODY_OPEN,
        BODY_CLOSE,
        HEADER,
        TU,
        NONE
    }

    /// <summary>
    /// Represents a tag in TMX file.
    /// </summary>
    public struct TmxTag
    {
        /// <summary>
        /// TagType of the TMX tag.
        /// </summary>
        public TmxTagType TmxTagType;
        /// <summary>
        /// Outer XML of the TMX tag.
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// Represents a reader that can read a sequential series of tags from a TMX file.
    /// </summary>
    public class TmxReader: IDisposable
    {

        private StreamReader TmxStreamReader;
        private bool EOF = false;
        private string tempStr = String.Empty;
        TmxTag CurrentTmxTag;

        /// <summary>
        /// The complete path of the TMX file to be read.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of TmxReader class for the specified TMX file.
        /// </summary>
        /// <param name="fileName">The complete path of the TMX file to be read.</param>
        public TmxReader(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("Could not find the TMX file: '{0}'", fileName));

            FileName = fileName;
            TmxStreamReader = new StreamReader(fileName);
        }

        #region IDisposable Members
        /// <summary>
        /// Releases all the resources used by TmxReader.
        /// </summary>
        public void Dispose()
        {
            TmxStreamReader.Close();
            TmxStreamReader.Dispose();
        }

        #endregion

        /// <summary>
        /// A collection of TMX tags.
        /// </summary>
        public IEnumerable<TmxTag> TmxTags
        {
            get
            {
                while (!EOF)
                {
                    yield return GetNextTag();
                }
                yield break;
            }
        }

        /// <summary>
        /// Reads and returns the next tag in the TMX file
        /// </summary>
        /// <returns>The next tag in the TMX file.</returns>
        private TmxTag GetNextTag()
        {
            string line = null;
            StringBuilder sb = new StringBuilder();
            string lineupper = String.Empty;
            
            int startIndex = -1;
            int endIndex = -1;
            TmxTagType TmxTagType = TmxTagType.NONE;
            CurrentTmxTag.TmxTagType = TmxTagType.NONE;
            CurrentTmxTag.Value = String.Empty;

            while ((line = TmxStreamReader.ReadLine()) != null || tempStr.Length > 0)
            {
                startIndex = -1;
                endIndex = -1;
                line = tempStr + line;
                lineupper = line.ToUpperInvariant();

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("<?XML "))
                {
                    startIndex = lineupper.IndexOf("<?XML ");
                    TmxTagType = TmxTagType.XML;
                }

                if (endIndex == -1 && lineupper.Contains("?>"))
                {
                    endIndex = lineupper.IndexOf("?>") + 2;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("<!DOCTYPE "))
                {
                    startIndex = lineupper.IndexOf("<!DOCTYPE ");
                    TmxTagType = TmxTagType.DOCTYPE;
                }

                if (endIndex == -1 && lineupper.Contains(">") && TmxTagType == TmxTagType.DOCTYPE)
                {
                    endIndex = lineupper.IndexOf(">") + 1;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("<TMX "))
                {
                    startIndex = lineupper.IndexOf("<TMX ");
                    TmxTagType = TmxTagType.TMX_OPEN;
                }

                if (endIndex == -1 && lineupper.Contains(">") && TmxTagType == TmxTagType.TMX_OPEN)
                {
                    endIndex = lineupper.IndexOf(">") + 1;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("<HEADER"))
                {
                    startIndex = lineupper.IndexOf("<HEADER");
                    TmxTagType = TmxTagType.HEADER;
                }

                if (endIndex == -1 && lineupper.Contains("</HEADER>") && TmxTagType == TmxTagType.HEADER)
                {
                    endIndex = lineupper.IndexOf("</HEADER>") + 9;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("<BODY"))
                {
                    startIndex = lineupper.IndexOf("<BODY");
                    TmxTagType = TmxTagType.BODY_OPEN;
                }

                if (endIndex == -1 && lineupper.Contains(">") && TmxTagType == TmxTagType.BODY_OPEN)
                {
                    endIndex = lineupper.IndexOf(">") + 1;
                }

                if (startIndex == -1 && endIndex == -1 && (lineupper.Contains("<TU ") || lineupper.Contains("<TU>")))
                {
                    startIndex = lineupper.IndexOf("<TU");
                    TmxTagType = TmxTagType.TU;
                }

                if (endIndex == -1 && lineupper.Contains("</TU>") && TmxTagType == TmxTagType.TU)
                {
                    endIndex = lineupper.IndexOf("</TU>") + 5;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("</BODY>"))
                {
                    startIndex = lineupper.IndexOf("</BODY>");
                    endIndex = startIndex + 7;
                    TmxTagType = TmxTagType.BODY_CLOSE;
                }

                if (startIndex == -1 && endIndex == -1 && lineupper.Contains("</TMX>"))
                {
                    startIndex = lineupper.IndexOf("</TMX>");
                    endIndex = startIndex + 6;
                    TmxTagType = TmxTagType.TMX_CLOSE;
                }

                if (startIndex == -1 && endIndex == -1)
                {
                    sb.Append(line);
                }
                else if (startIndex != -1 && endIndex == -1)
                {
                    sb.Remove(0, sb.ToString().Length);
                    sb.Append(line.Substring(startIndex));
                }
                else if (startIndex != -1 && endIndex != -1)
                {
                    sb.Remove(0, sb.ToString().Length);
                    sb.Append(line.Substring(startIndex, endIndex - startIndex));
                }
                else if (startIndex == -1 && endIndex != -1)
                {
                    sb.Append(line.Substring(0, endIndex));
                }

                if (endIndex != -1)
                {
                    tempStr = line.Substring(endIndex);
                    CurrentTmxTag.TmxTagType = TmxTagType;
                    CurrentTmxTag.Value = sb.ToString();
                    if (TmxStreamReader.EndOfStream && tempStr.Trim().Length == 0)
                    {
                        EOF = true;
                    }
                    break;
                }

                
            }
            if (TmxStreamReader.EndOfStream && tempStr.Trim().Length == 0)
            {
                EOF = true;
            }
            return CurrentTmxTag;
        }
    }
}
