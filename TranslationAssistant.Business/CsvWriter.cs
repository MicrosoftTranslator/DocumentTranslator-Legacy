//Simple CSV writer class.

using System;
using System.IO;
using System.Text;

namespace TranslationAssistant.Business
{
    public class CsvWriter : IDisposable
    {
        public enum Disposition { translate, DNT, attribute }

        private StreamWriter CsvStream;

        /// <summary>
        /// Creates and initializes a CSV file for writing
        /// </summary>
        /// <param name="TmxFilename">TMX file name</param>
        public CsvWriter(string filename)
        {
            this.CsvStream = new StreamWriter(filename, false, Encoding.UTF8);
            WriteHeader();
            return;
        }


        public void WriteElement(string Element, XMLTranslationManager.Properties props)
        {
            this.CsvStream.Write("\"{0}\",", CSVEncode(Element));
            this.CsvStream.Write("\"{0}\",", CSVEncode(props.Type));
            this.CsvStream.Write("\"{0}\"\n", CSVEncode(props.Disposition));
        }

        private string CSVEncode(string segment)
        {
            segment = segment.Replace("\"", "\"\"");
            return segment;
        }

        private string statusmessage(Disposition disposition)
        {
            switch (disposition)
            {
                case Disposition.translate:
                    return ("translate");
                case Disposition.DNT:
                    return ("do not translate");
                case Disposition.attribute:
                    return ("attribute");
                default:
                    return("");
            }
        }

        private void WriteHeader()
        {
            this.CsvStream.WriteLine("\"Name\",\"Type\",\"Disposition\"");
        }

        public void Dispose()
        {
            this.CsvStream.Flush();
            this.CsvStream.Close();
            this.CsvStream.Dispose();
        }
    }
}
