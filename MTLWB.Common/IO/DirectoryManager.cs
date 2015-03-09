using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Collections;
using System.Security.AccessControl;
using System.Xml;
using MTLWB.Common.Log;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Provides static methods for IO related operations
    /// </summary>
    public static class DirectoryManager
    {

        /// <summary>
        /// Creates the exact directory structure as of input directory into output directory.
        /// </summary>
        /// <param name="inputDirectory">Location of input directory</param>
        /// <param name="outputDirectory">Location of output directory</param>
        public static void CreateOutputDirectory(string inputDirectory, string outputDirectory)
        {
            CreateOutputDirectory(inputDirectory, outputDirectory, null);
        }

        /// <summary>
        /// Creates the exact directory structure as of input directory into output directory for each language.
        /// </summary>
        /// <param name="inputDirectory">Location of input directory</param>
        /// <param name="outputDirectory">Location of output directory</param>
        /// <param name="targetLanguages">List of target languages for which the directory structure is to be created</param>
        public static void CreateOutputDirectory(string inputDirectory, string outputDirectory, List<string> targetLanguages)
        {
            if (string.IsNullOrEmpty(inputDirectory))
                throw new ArgumentNullException("inputDirectory");
            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentNullException("outputDirectory");
            if (!Directory.Exists(inputDirectory))
                throw new DirectoryNotFoundException(string.Format("Could not find the input directory {0}", inputDirectory));
            if (!Directory.Exists(outputDirectory))
                throw new DirectoryNotFoundException(string.Format("Could not find the output directory {0}", outputDirectory));

            Logger.Log(LogType.Status, "Creating output directory structure");

            DirectoryInfo currDir;
            if (!inputDirectory.EndsWith("\\"))
                inputDirectory += "\\";

            string tempDir = Path.Combine(outputDirectory, "tempfiles");
            currDir = new DirectoryInfo(tempDir);
            if (currDir.Exists)
                EmptyDirectory(tempDir);
            else
                currDir.Create();

            string[] inputDirs = Directory.GetDirectories(inputDirectory, "*", SearchOption.AllDirectories);
            CreateSubDirectories(inputDirectory, inputDirs, currDir);

            if (targetLanguages == null)
            {
                CreateSubDirectories(inputDirectory, inputDirs, new DirectoryInfo(outputDirectory));
            }
            else
            {
                foreach (string targetLanguage in targetLanguages)
                {
                    string langDir = Path.Combine(outputDirectory, targetLanguage);
                    currDir = new DirectoryInfo(langDir);
                    if (currDir.Exists)
                        EmptyDirectory(langDir);
                    else
                        currDir.Create();

                    CreateSubDirectories(inputDirectory, inputDirs, currDir);
                }
            }
        }

        private static void CreateSubDirectories(string inputRootDir, string[] inputSubdirs, DirectoryInfo outputDir)
        {
            foreach (string dir in inputSubdirs)
            {
                outputDir.CreateSubdirectory(dir.Replace(inputRootDir, ""));
            }
        }

        private static void EmptyDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            foreach (string dir in dirs)
            {
                EmptyDirectory(dir);
            }
        }

        /// <summary>
        /// Returns the path of the template file for the specified language for the dictinary creation task.
        /// </summary>
        /// <param name="language">2 character language code</param>
        /// <returns>Matching template file path</returns>
        public static string GetTemplateFilePath(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");

            switch (language.ToLower())
            {
                case "zh-cn":
                case "zh-chs":
                    language = "zh-cn";
                    break;
                case "zh-tw":
                case "zh-cht":
                    language = "zh-tw";
                    break;
                default:
                    language = language.Substring(0, 2);
                    break;
            }
            string templateFile = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\" + language + ".xml";
            if (!File.Exists(templateFile))
                templateFile = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\Default.xml";

            return templateFile;
        }

    }
}
