using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using MTLWB.Common;

namespace MTLWB.HxS
{

    /// <summary>
    /// Provides methods to compile, decompile HxS files and perform markup on the decompiled HTML files.
    /// </summary>
    public class HxsManager
    {

        private enum CompActions
        {
            Compile,
            Decompile
        }

        private string[] HxcompPaths = 
        { 
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\microsoft shared\Help 2.0 Compiler\HxComp.exe",
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Archive\HelpIntegration\hxcomp.exe",
            ConfigurationManager.AppSettings["hxcompPath"].EndsWith("hxcomp.exe", StringComparison.InvariantCultureIgnoreCase) ? ConfigurationManager.AppSettings["hxcompPath"] : Path.Combine(ConfigurationManager.AppSettings["hxcompPath"], "hxcomp.exe")
        };

        public int LastExitCode { get; private set; }

        private string HxcompPath = string.Empty;
        private string SourceHxs = string.Empty;
        private string OutputDirectory = string.Empty;
        private string SourceLanguage = string.Empty;
        private string TargetLanguage = string.Empty;
        private List<FileInfo> files;

        const string SINGLEPTRN = "*.htm*";

        /// <summary>
        /// Initializes a new instance of HxsManager class
        /// </summary>
        /// <param name="sourceHxs">The complete path of the source HxS file.</param>
        /// <param name="outputDirectory">The complete path of the output directory.</param>
        /// <param name="sourceLanguage">A string representing the language code of the translation text.</param>
        /// <param name="targetLanguage">A string representing the language code to translate the text into.</param>
        public HxsManager(string sourceHxs, string outputDirectory, string sourceLanguage, string targetLanguage)
        {
            foreach (string path in HxcompPaths)
            {
                if (File.Exists(path))
                {
                    HxcompPath = path;
                    break;
                }
            }
            if (HxcompPath.Length == 0)
                throw new FileNotFoundException("Hxcomp utility not found. Make sure Help 2.0 compiler is installed and correct path is specified in config file.");

            if(!File.Exists(sourceHxs))
                throw new FileNotFoundException(string.Format("File [{0}] does not exist!",sourceHxs));

            if (!Directory.Exists(outputDirectory))
                throw new DirectoryNotFoundException(string.Format("Output directory [{0}] does not exist!", outputDirectory));

            this.SourceHxs = sourceHxs;
            this.OutputDirectory = outputDirectory;
            this.SourceLanguage = sourceLanguage;
            this.TargetLanguage = targetLanguage;
        }

        /// <summary>
        /// Decompiles the source HxS file to output directory using Help 2.0 (hxcomp.exe) utility.
        /// </summary>
        public void DecompileHxs()
        {
            
            ProcessCommand(CompActions.Decompile);
        }

        /// <summary>
        /// Compiles the marked-up and translated files back to HxS file using Help 2.0 (hxcomp.exe) utility.
        /// </summary>
        public void CompileHxs()
        {
            ProcessCommand(CompActions.Compile);
        }

        /// <summary>
        /// Performs the required markup on decompiled HTML files for MSDN publishing.
        /// </summary>
        /// <param name="isBilingual">Value indicating whether the output HxS file is bilingual or not.</param>
        /// <param name="translationType">Value to be set for TranslationType attribute in HTML files.</param>
        /// <param name="communityContent">Value to be set for CommunityContent attribute in HTML files.</param>
        /// <returns>List of FileInfo objects for all marked-up files</returns>
        public List<FileInfo> markupHtml(bool isBilingual, TranslationType translationType, bool communityContent)
        {
            files = getHtmlFileList(OutputDirectory, SINGLEPTRN);

            //Call MT Markup to get tagged files.
            Markup objMarkup = new Markup();
            objMarkup.ProcessMarkup(isBilingual, files, OutputDirectory, SourceLanguage, TargetLanguage, translationType, communityContent);

            return files;
        }

        private List<FileInfo> getHtmlFileList(string sourceDir, string searchPattern)
        {
            //Get search pattern 
            string[] fileExtension = searchPattern.Split(';');
            //Declare list 
            List<FileInfo> fiSourceList = new List<FileInfo>();

            //Create instance of direcotry info
            DirectoryInfo diSourceList = new DirectoryInfo(sourceDir);

            //Get All File from decompile folder
            for (int i = 0; i < fileExtension.Length; i++)
            {
                foreach (FileInfo fi in diSourceList.GetFiles(fileExtension[i], SearchOption.AllDirectories))
                {
                    fiSourceList.Add(fi);
                }
            }
            //return FileList
            return fiSourceList;
        }

        private void ProcessCommand(CompActions Action)
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo(HxcompPath);
            if (Action == CompActions.Compile)
            {
                info.Arguments = String.Format("-p \"{0}.HxC\"", Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(SourceHxs)));
            }
            if (Action == CompActions.Decompile)
            {
                info.Arguments = String.Format("-u \"{0}\" -d \"{1}\"", SourceHxs, OutputDirectory);
            }
            info.Verb = "Open";
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;
            process.StartInfo = info;
            process.Start();
            string str = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.HasExited)
            {
                this.LastExitCode = process.ExitCode;
            }
        }

    }
}
