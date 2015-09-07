//Simple CSV writer class.

using System;
using System.IO;
using System.Text;

namespace Mts.Common.Tmx
{
    public class CsvWriter : IDisposable
    {
        private StreamWriter CsvStream;

        /// <summary>
        /// Creates and initializes a CSV file for writing
        /// </summary>
        /// <param name="TmxFilename">TMX file name</param>
        public CsvWriter(string filename, string sourcelanguage, string targetlanguage)
        {
            this.CsvStream = new StreamWriter(filename, false, Encoding.UTF8);
            WriteHeader(sourcelanguage);
            return;
        }


        public void WriteSegment(string sourcesegment, string targetsegment, TmxWriter.TUError tustatus)
        {
            this.CsvStream.Write("\"{0}\",", CSVEncode(sourcesegment));
            this.CsvStream.Write("\"{0}\",", CSVEncode(targetsegment));
            this.CsvStream.WriteLine("\"{0}\"", statusmessage(tustatus));
        }

        private string CSVEncode(string segment)
        {
            return segment.Replace("\"", "\"\"");
        }

        private string statusmessage(TmxWriter.TUError tustatus)
        {
            switch (tustatus)
            {
                case TmxWriter.TUError.good:
                    return ("Good");
                case TmxWriter.TUError.lengthratio:
                    return ("Length ratio exceeded");
                case TmxWriter.TUError.sentencecountmismatch:
                    return ("Sentence count mismatch");
                case TmxWriter.TUError.tagging:
                    return ("Sentence contains tags");
                default:
                    return("");
            }
        }

        private void WriteHeader(string sourcelanguage)
        {
            this.CsvStream.WriteLine("\"Source language\",\"Target language\",\"Error type\"");
        }

        public void Dispose()
        {
            this.CsvStream.Flush();
            this.CsvStream.Close();
            this.CsvStream.Dispose();
        }
    }
}
