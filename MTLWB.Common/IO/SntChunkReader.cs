using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Represents a reader that can read a sequential series of chunks of sentences from a SNT file.
    /// </summary>
    public class SntChunkReader : IDisposable
    {
        FileStream SntFileStream;
        StreamReader ChunkReader;

        /// <summary>
        /// Gets a value that indicates whether the current position of reader is at the end of the SNT file stream.
        /// </summary>
        public bool EOF
        {
            private set;
            get;
        }

        string tempLine = String.Empty;
        private int _lowerBound = 1;
        private int _upperBount = 0;
        List<string> sntChunk = new List<string>();

        /// <summary>
        /// Initializes a new instance of the SntChunkReader class for the specified SNT file.
        /// </summary>
        /// <param name="sntFile">The complete file path of the SNT file to be read.</param>
        public SntChunkReader(string sntFile)
        {
            if (string.IsNullOrEmpty(sntFile))
                throw new ArgumentNullException("sntFile");
            if (!File.Exists(sntFile))
                throw new FileNotFoundException(string.Format("Snt file ({0}) not found.", sntFile));

            SntFileStream = new FileStream(sntFile, FileMode.Open, FileAccess.Read);
            ChunkReader = new StreamReader(SntFileStream, Encoding.Unicode);
            EOF = false;
        }

        /// <summary>
        /// Returns an enumerator to iterate through SNT chunks
        /// </summary>
        public IEnumerable<string[]> ChunkCollection
        {
            get
            {
                while (!EOF)
                {
                    yield return ReadNextChunk();
                }
                yield break;
            }
        }

        /// <summary>
        /// Reads the next chunk of sentences from the SNT file
        /// </summary>
        /// <returns>Next chunk of sentences in the SNT file</returns>
        public string[] ReadNextChunk()
        {
            int prevSize = 0;
            string[] chunk;
            string currLine = String.Empty;
            while (!ChunkReader.EndOfStream)
            {

                currLine = ChunkReader.ReadLine();
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] bytesDate = encoding.GetBytes(currLine);
                prevSize = prevSize + bytesDate.Length;
                if (prevSize <= Statics.CHUNK_SIZE)
                {
                    sntChunk.Add(currLine);
                    currLine = string.Empty;
                }
                else
                {
                    break;
                }
            }
            chunk = new string[sntChunk.Count];
            sntChunk.CopyTo(chunk);
            sntChunk.Clear();
            if (currLine.Length>0)
            {
                sntChunk.Add(currLine);
            }
            if (ChunkReader.EndOfStream && currLine.Length == 0)
            {
                EOF = true;
            }
            _upperBount = _lowerBound + chunk.Length - 1;

            return chunk;

        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by SntReader.
        /// </summary>
        public void Dispose()
        {
            ChunkReader.Close();
            SntFileStream.Close();
            ChunkReader.Dispose();
            SntFileStream.Dispose();
        }

        #endregion
    }
}
