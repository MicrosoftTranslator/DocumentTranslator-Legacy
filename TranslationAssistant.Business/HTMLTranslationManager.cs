/*
 * Translate HTML files.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace TranslationAssistant.Business
{
    class HTMLTranslationManager
    {
        public static int DoTranslation(string htmlfilename, string fromlanguage, string tolanguage)
        {
            string htmldocument = File.ReadAllText(htmlfilename);
            string htmlout = string.Empty;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmldocument);
            htmlDoc.DocumentNode.SetAttributeValue("lang", TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(tolanguage));
            var title = htmlDoc.DocumentNode.SelectSingleNode("//head//title");
            title.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(title.InnerHtml, fromlanguage, tolanguage, "text/html");
            var body = htmlDoc.DocumentNode.SelectSingleNode("//body");
            if (body.InnerHtml.Length < 10000)
            {
                body.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(body.InnerHtml, fromlanguage, tolanguage, "text/html");
            }
            else
            {
                List<HtmlNode> nodes = new List<HtmlNode>();
                AddNodes(body.FirstChild, ref nodes);

                Parallel.ForEach(nodes, (node) =>
                    {
                        if (node.InnerHtml.Length > 10000)
                        {
                            throw new Exception("Child node with a length of more than 10000 characters encountered.");
                        }
                        node.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(node.InnerHtml, fromlanguage, tolanguage, "text/html");
                    });
            }
            htmlDoc.Save(htmlfilename, Encoding.UTF8);
            return 1;
        }

        /// <summary>
        /// Add nodes of size smaller than 10000 characters to the list, and recurse into the bigger ones.
        /// </summary>
        /// <param name="rootnode">The node to start from</param>
        /// <param name="nodes">Reference to the node list</param>
        private static void AddNodes(HtmlNode rootnode, ref List<HtmlNode> nodes)
        {
            string[] DNTList = { "script", "#text", "code", "col", "colgroup", "embed", "em", "#comment", "image", "map", "media", "meta", "source", "xml"};  //DNT - Do Not Translate - these nodes are skipped.
            HtmlNode child = rootnode;
            while (child != rootnode.LastChild)
            {
                if (!DNTList.Contains(child.Name.ToLowerInvariant())) {
                    if (child.InnerHtml.Length > 10000)
                    {
                        AddNodes(child.FirstChild, ref nodes);
                    }
                    else
                    {
                        if (child.InnerHtml.Trim().Length != 0) nodes.Add(child);
                    }
                }
                child = child.NextSibling;
            }
        }

    }
}
