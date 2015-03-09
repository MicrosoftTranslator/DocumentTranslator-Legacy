using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using MTLWB.Common;

namespace MTLWB.HxS
{
    internal class MTProcessor
    {
        internal string Content = null;

        public static void MarkupFile(FileInfo sourceFile, string targetDir, string mtSourceDir,
            string sourceLang, string outputLang, List<string> expressions, TranslationType translationType, bool communityContent)
        {
            // Pull in the source file as an HTMLDocument.
            CultureInfo ci = new CultureInfo(sourceLang);
            CultureInfo ciTarget = new CultureInfo(outputLang);
            //Guid pageID = Guid.NewGuid();

            HtmlDocument sourceDoc;
            HtmlDocument outputDoc;

            
            sourceDoc = new HtmlDocument(targetDir + "\\" + sourceFile, ci, MarkupMode.Source, expressions, translationType, communityContent);
            sourceDoc.OptionWriteEmptyNodes = true;
            sourceDoc.OptionAutoCloseOnEnd = true;

            outputDoc = new HtmlDocument(targetDir + "\\" + sourceFile, ciTarget, MarkupMode.Output, expressions, translationType, communityContent);
            outputDoc.OptionWriteEmptyNodes = true;
            outputDoc.OptionAutoCloseOnEnd = true;

            //TODO: Fix this in the HtmlDocument class!
            // Clean up step below -- replace in strings -- make sure all sentenceId references are camel cased.

            // Write the output file to disk.
            File.WriteAllText(targetDir + "\\" + sourceFile.Name, outputDoc.DocumentNode.OuterHtml.Replace("sentenceid", "sentenceId"), Encoding.UTF8);

            // Write the altered source file to disk.
            if (!Directory.Exists(mtSourceDir))
                Directory.CreateDirectory(mtSourceDir);
            File.WriteAllText(mtSourceDir + "\\" + sourceFile.Name, sourceDoc.DocumentNode.OuterHtml.Replace("sentenceid", "sentenceId").Replace("<NoLocCodeContent>", "").Replace("</NoLocCodeContent>", ""), Encoding.UTF8);
        }
    }
}