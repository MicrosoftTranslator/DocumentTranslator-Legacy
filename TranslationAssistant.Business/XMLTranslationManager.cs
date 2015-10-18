// Translate XML files, and related methods.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace TranslationAssistant.Business
{
    
    public class XMLTranslationManager
    {
        
        /// <summary>
        /// Holds the properties of an element or attribute
        /// </summary>
        public struct Properties
        {
            public string Type;         //the type of the node: element or attrinbute
            public string Disposition;  //translate or not translate
        }

        /// <summary>
        /// Translate an XML file using the element characteristics read from a CSV file
        /// </summary>
        /// <param name="xmlfilename">The XML file to translate</param>
        /// <param name="csvfilename">The CSV that contains the element disposition</param>
        /// <param name="fromlanguage">Language to translate from</param>
        /// <param name="tolanguage">Language to translate to</param>
        public static int DoTranslation(string xmlfilename, string csvfilename, string fromlanguage, string tolanguage)
        {
            SortedList<string, Properties> elementlist = new SortedList<string, Properties>();
            List<string> ToTranslate = new List<string>();

            elementlist = ReadElementsFromCSV(csvfilename);

            //Read elements to translate into List
            using (XmlReader xmlreader = XmlReader.Create(xmlfilename))
            {
                while (xmlreader.Read())
                {
                    if ((xmlreader.IsStartElement()) && (elementlist.ContainsKey(xmlreader.Name)))
                    {
                        ToTranslate.Add(xmlreader.ReadInnerXml());
                    }
                }
            }

            //Do the actual translation
            List<string> FromTranslate = TranslateList(ToTranslate, fromlanguage, tolanguage);

            //Write into target XML file
            using (XmlReader xmlreader = XmlReader.Create(xmlfilename))
            {
                string xmloutfilename = Path.GetFileNameWithoutExtension(xmlfilename) + "." + tolanguage + "." + Path.GetExtension(xmlfilename);
                File.Delete(xmloutfilename);
                XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                xmlwritersettings.Encoding = Encoding.UTF8;
                xmlwritersettings.CloseOutput = true;
                int index = 0;

                using (XmlWriter xmlwriter = XmlWriter.Create(xmloutfilename, xmlwritersettings))
                {
                    while (xmlreader.Read())
                    {
                        if (xmlreader.ReadState == ReadState.Initial) continue;
                        if ((xmlreader.IsStartElement()) && (elementlist.ContainsKey(xmlreader.Name)))
                        {
                            xmlwriter.WriteRaw(FromTranslate[index]);
                            index++;
                        }
                        else
                        {
                            //this doesn't work at all. It advances the reader to the next element at the same level, while we need to look into the element until we find the child that needs translation. 
                            xmlwriter.WriteNode(xmlreader, true);
                        }
                    }
                }
            }
            return FromTranslate.Count;



        }

        /// <summary>
        /// Translates a List of arbitrary length
        /// </summary>
        /// <param name="ToTranslate">List to translate</param>
        /// <param name="fromlanguage">Language to translate from</param>
        /// <param name="tolanguage">Language to translate to</param>
        /// <returns>Translated list</returns>
        private static List<string> TranslateList(List<string> ToTranslate, string fromlanguage, string tolanguage)
        {
            List<string> FromTranslate = new List<string>();
            List<string> packet = new List<string>(5000);
            int packetsize = 0;
            int index = 0;

            while (index < ToTranslate.Count)
            {
                packet.Add(ToTranslate[index]);
                packetsize += ToTranslate[index].Length;
                if ((packetsize > 5000) || (index >= (ToTranslate.Count - 1)))
                {
                    string[] tresult = TranslationServices.Core.TranslationServiceFacade.TranslateArray(packet.ToArray(), fromlanguage, tolanguage);
                    FromTranslate.AddRange(tresult);
                    packetsize = 0;
                    packet.Clear();
                }
                index++;
            }

            return FromTranslate;
        }

        private static SortedList<string, Properties> ReadElementsFromCSV(string csvfilename)
        {
            SortedList<string, Properties> elementlist = new SortedList<string, Properties>();
            Properties props = new Properties();
            char[] separator = {','};

            using (StreamReader csvfile = new StreamReader(csvfilename))
            {
                while (!csvfile.EndOfStream)
                {
                    string line = csvfile.ReadLine();
                    line = line.Replace('\"', ' ');
                    string[] cells = line.Split(separator, 3);
                    props.Type = cells[1].Trim();
                    props.Disposition = cells[2].Trim();
                    if (props.Disposition.ToLowerInvariant() == "translate")
                    {
                        try { elementlist.Add(cells[0].Trim(), props); }
                        catch { };
                    }
                }
            }
            return elementlist;
        }

        public static void SaveElementsToCSV(string xmlfilename, string csvfilename)
        {
            SortedList<string, Properties> elementlist = new SortedList<string, Properties>();
            Properties props = new Properties();

            using (XmlReader xmlreader = XmlReader.Create(xmlfilename))
            {

                while (xmlreader.Read())
                {
                    if (xmlreader.IsStartElement())
                    {
                        props.Type = "element";
                        props.Disposition = "do not translate";
                        try { elementlist.Add(xmlreader.Name, props); }
                        catch { }
                    }
                    if (xmlreader.HasAttributes)
                    {
                        for (int attIndex = 0; attIndex < xmlreader.AttributeCount; attIndex++)
                        {
                            xmlreader.MoveToAttribute(attIndex);
                            props.Type = "attribute";
                            try { elementlist.Add(xmlreader.Name, props); }
                            catch { };
                            props.Type = "attributevalue";
                            try {  elementlist.Add(xmlreader.Value, props); }
                            catch { };
                        }
                    }
                }

            }

            using (CsvWriter csvwriter = new CsvWriter(csvfilename))
            {
                foreach (var element in elementlist)
                {
                    csvwriter.WriteElement(element.Key, element.Value);
                }
            }
            return;
        }

    }
}
