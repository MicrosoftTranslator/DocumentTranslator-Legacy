using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration; 
using System.Collections.Generic;
using MTLWB.Common;

namespace MTLWB.HxS
{

    internal class Markup
    {   

        #region ProcessMarkup
        /// <summary>
        /// Performs markup tagging on html files for MSDN publishing.
        /// </summary>
        /// <param name="isBilingual">value indicates whether the output HXS is to be bilingual or not</param>
        /// <param name="files">List of html files on which tagging is to be performed</param>
        /// <param name="SourceDir">Input directory</param>
        /// <param name="sourceLocale">source langugage code</param>
        /// <param name="targetLocale">target language code</param>
        /// <param name="translationType">Indicates the value for TranslationType attribute in html files</param>
        /// <param name="communityContent">Indicates the value (0/1) for CommunityContent attribute in html files</param>
        public void ProcessMarkup(bool isBilingual,List<FileInfo> files,string SourceDir, string sourceLocale, string targetLocale, TranslationType translationType, bool communityContent)
        {
            //Create Expresssion 
            DirectoryInfo parentDirectory = new DirectoryInfo(SourceDir);            

            if (isBilingual)
            {
                string mtMarkupFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\MTMarkup.config";
                List<string> queryStrings = new List<string>();

                XmlDocument config = new XmlDocument();
                config.Load(mtMarkupFile);
                foreach (XmlNode xn in config.SelectNodes("descendant::Query"))
                {
                    queryStrings.Add(xn.InnerText);
                }

                //if (fileInfo.Count<=0)
                //{
                //    logSW.WriteLine("Specified source directory {0} is invalid.\n", SourceDir);
                //    return;
                //}
                

                // Initialize static data (need to do this only once per session)
                Statics.Init("Settings.ini", System.AppDomain.CurrentDomain.BaseDirectory, new Uri(System.AppDomain.CurrentDomain.BaseDirectory));

                int fileCount = 0;

                DirectoryInfo sourceDirectory;

                //logSW.WriteLine("Beginning markup for project {0}", SourceDir);
                fileCount = 0;

                //For each html file in the source directory
                foreach (FileInfo fi in files)
                {
                    fileCount++;
                    // Begin the markup!
                    //logSW.WriteLine(fileCount + ". " + fi.Name + ": Beginning Translation");
                    
                    MTProcessor.MarkupFile(fi, fi.FullName.Substring(0,fi.FullName.LastIndexOf("\\")), SourceDir + "\\MTSource\\html",
                    sourceLocale, targetLocale, queryStrings, translationType, communityContent);
                    //logSW.WriteLine(fi.Name + ": Translation Complete");
                }
                //logSW.WriteLine("{0} files processed\n", fileCount);                

                // Copy the .hxt and .hxc files to the MTSource directory.
                FileInfo[] hxtFile = parentDirectory.GetFiles("*.HxT");
                if (hxtFile.Length > 0)
                {
                    string hxtFileName = hxtFile[0].Name;
                    if (!File.Exists(parentDirectory.FullName + "\\MTSource\\" + hxtFileName))
                        hxtFile[0].CopyTo(parentDirectory.FullName + "\\MTSource\\" + hxtFileName);
                }

                FileInfo[] hxcFile = parentDirectory.GetFiles("*.HxC");
                if (hxcFile.Length > 0)
                {
                    string hxcFileName = hxcFile[0].Name;
                    if (!File.Exists(parentDirectory.FullName + "\\MTSource\\" + hxcFileName))
                        hxcFile[0].CopyTo(parentDirectory.FullName + "\\MTSource\\" + hxcFileName);
                    // Update the HxC file with the new locale.
                    string hxcFileText = File.ReadAllText(hxcFile[0].FullName);
                    hxcFileText = hxcFileText.Replace("LangId=\"1033\"", "LangId=\"1030\"");
                    File.WriteAllText(hxcFile[0].FullName, hxcFileText);
                }

                //Update the HxF file to include the files in the MTSource directory in the build.
                FileInfo[] hxfFile = null;
                hxfFile = parentDirectory.GetFiles("*.HxF");
                if (hxfFile.Length != 0)
                {
                    string hxfFileText = File.ReadAllText(hxfFile[0].FullName);
                    hxfFileText = hxfFileText.Replace("</HelpFileList>", "<File Url=\"MTSource\\*.*\" /><File Url=\"MTSource\\html\\*.htm\" /></HelpFileList>");
                    File.WriteAllText(hxfFile[0].FullName, hxfFileText);
                }

                //logSW.WriteLine(" ");
                sourceDirectory = new DirectoryInfo(SourceDir + "\\html");

                //logSW.WriteLine("Beginning markup for project {0}", SourceDir);
                fileCount = 0;            
                //logSW.WriteLine("{0} files processed\n", fileCount);

                //logSW.WriteLine(" ");
            }
            else
            {
                string[] fileExtension = { "*.HxF", "*.Hxt" };
                string strTarget = @"\S+\.html?";
                string strReplace = "Url=\"";
                foreach (string extFilter in fileExtension)
                {
                    FileInfo[] hxfFile = parentDirectory.GetFiles(extFilter);
                    if (hxfFile.Length != 0)
                    {
                        string hxfFileText = File.ReadAllText(hxfFile[0].FullName);
                        hxfFileText = System.Text.RegularExpressions.Regex.Replace(hxfFileText, strTarget, strReplace);
                        if (extFilter.ToLower() == "*.hxf")
                            hxfFileText = hxfFileText.Replace("<File Url=\"\" /></HelpFileList>", "<File Url=\"MTSource\\html\\*.htm\" /></HelpFileList>");
                        File.WriteAllText(hxfFile[0].FullName, hxfFileText);
                        hxfFile = null;
                    }
                }
            } 
        }
        #endregion ProcessMarkup
        
    }
}
