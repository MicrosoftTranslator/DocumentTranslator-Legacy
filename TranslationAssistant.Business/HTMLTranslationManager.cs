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
        private static readonly int maxRequestSize = TranslationServices.Core.TranslationServiceFacade.Maxrequestsize;

        public static int DoTranslation(string htmlfilename, string fromlanguage, string tolanguage)
        {
            string htmldocument = File.ReadAllText(htmlfilename);
            string htmlout = string.Empty;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmldocument);
            htmlDoc.DocumentNode.SetAttributeValue("lang", TranslationServices.Core.TranslationServiceFacade.LanguageNameToLanguageCode(tolanguage));
            var title = htmlDoc.DocumentNode.SelectSingleNode("//head//title");
            if (title != null) title.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(title.InnerHtml, fromlanguage, tolanguage, TranslationServices.Core.TranslationServiceFacade.ContentType.HTML);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//body");
            if (body != null)
            {
                if (body.InnerHtml.Length < maxRequestSize)
                {
                    body.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(body.InnerHtml, fromlanguage, tolanguage, TranslationServices.Core.TranslationServiceFacade.ContentType.HTML);
                }
                else
                {
                    List<HtmlNode> nodes = new List<HtmlNode>();
                    AddNodes(body.FirstChild, ref nodes);

                    Parallel.ForEach(nodes, (node) =>
                        {
                            node.InnerHtml = TranslationServices.Core.TranslationServiceFacade.TranslateString(node.InnerHtml, fromlanguage, tolanguage, TranslationServices.Core.TranslationServiceFacade.ContentType.HTML);
                        });
                }
            }
            htmlDoc.Save(htmlfilename, Encoding.UTF8);
            return 1;
        }

        /// <summary>
        /// Add nodes of size smaller than maxRequestSize characters to the list, and recurse into the bigger ones.
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
                    if (child.InnerHtml.Length > maxRequestSize)
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
