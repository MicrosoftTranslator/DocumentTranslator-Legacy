// -
// <copyright file="TmxFile.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mts.Common.Tmx.Parser;

namespace Mts.Common.Tmx
{
    /// <summary>
    /// Wrapper class for Reading/Writing Tmx files.
    /// </summary>
    public class TmxFile
    {
        /// <summary>
        /// Field to keep track of internal counter.
        /// </summary>
        private int internalCounter;

        /// <summary>
        /// Field contains the input stream.
        /// </summary>
        private StreamReader streamReader;

        /// <summary>
        /// Field contains SntWriters for each language.
        /// </summary>
        private SntWriterContainer writers;

        /// <summary>
        /// Initializes a new instance of the TmxFile class.
        /// </summary>
        /// <param name="filePath">Specifies a base file path.</param>
        public TmxFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File not found in the specified path");
            }

            this.streamReader = new StreamReader(filePath);
            Debug.Assert(this.streamReader != null, "streamReader is null");

            this.writers = null;
            this.internalCounter = 0;
        }

        /// <summary>
        /// Initializes a new instance of the TmxFile class.
        /// </summary>
        /// <param name="stream">Specifies a input stream.</param>
        public TmxFile(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.streamReader = new StreamReader(stream);
            Debug.Assert(this.streamReader != null, "streamReader is null");
            this.writers = null;
            this.internalCounter = 0;
        }

        /// <summary>
        /// Gets or sets tmx file path.
        /// </summary>
        public string FilePath { get; set; }

        

        /// <summary>
        /// Saves content as sentence-by-sentence files
        /// </summary>
        /// <param name="outputFilePath">The base name and extension for the text files</param>
        /// <returns>Array of text file names.</returns>
        public string[] WriteToSNTFiles(string outputFilePath)
        {
            string folder = Path.GetDirectoryName(outputFilePath);
            string fileName = Path.GetFileNameWithoutExtension(outputFilePath);
            string extension = string.Empty;

            if (Path.HasExtension(outputFilePath))
            {
                extension = Path.GetExtension(outputFilePath);
            }

            string[] outputFiles = null;
            try
            {
                this.writers = new SntWriterContainer(Path.Combine(folder, fileName), extension);

                using (TmxReader rdr = new TmxReader(this.streamReader))
                {
                    foreach (TmxTag tmxTag in rdr.TmxTags)
                    {
                        switch (tmxTag.TmxTagType)
                        {
                            case TmxTagType.TU:
                                TmxDocument tmxtuDoc = TmxDocument.Create(tmxTag.Value, true);
                                this.ProcessTuElement(tmxtuDoc);
                                break;

                            ////case TmxTagType.XML:
                            ////case TmxTagType.DOCTYPE:
                            ////case TmxTagType.TMX_OPEN:
                            ////case TmxTagType.HEADER:
                            ////case TmxTagType.BODY_OPEN:
                            default:
                                break;
                        }
                    }
                }

                outputFiles = this.writers.Files;
            }
            catch
            {
                throw;         
            }
            finally
            {
                if (this.writers != null)
                {
                    this.writers.Dispose();
                    this.writers = null;
                }
            }

            return outputFiles;
        }

        /// <summary>
        /// Processes the TU element and write the TUV segment data into the output files.
        /// </summary>
        /// <param name="tmxtuElement">The TU element.</param>
        private void ProcessTuElement(TmxDocument tmxtuElement)
        {
            // Extract first level nodes of current <tu> element.
            TmxNodeCollection nodes = tmxtuElement.Nodes.FindByName(Token.TUV, true);
            List<KeyValuePair<string, TmxNodeCollection>> tuvNodeList = new List<KeyValuePair<string, TmxNodeCollection>>();

            // Iterate through the TUV nodes in the TU element and create a list of source and target language TUV nodes.
            // It is expected that a single TU element will contain TUV elements of only 2 languages.
            foreach (TmxNode node in nodes)
            {
                TmxElement element = (TmxElement)node;
                TmxAttribute langAttr = element.Attributes["lang"] ?? element.Attributes["xml:lang"];
                if (langAttr != null)
                {
                    tuvNodeList.Add(new KeyValuePair<string, TmxNodeCollection>( LanguageMapper.MapLanguage(langAttr.Value.ToLowerInvariant()), element.Nodes));
                }
            }

            // Create groups for the TUV nodes based on their language code.
            var tuvLangGroups = from tuvNode in tuvNodeList
                                group tuvNode by tuvNode.Key into tuvLangGroup
                                select tuvLangGroup;
            
            // If the TU element contains multiple TUV elements of the same language then we need to add duplicate
            // sentences for other language TUV elements in the final output file.
            // Basically we will create a Cartesian product of the different language TUV elements.
            // e.g. - Language1 = { a1 }
            //        Language2 = { b1, b2 }
            //        Language3 = { c1, c2 }
            // The following pairs are generated via the Cartesian product
            // => { a1, b1, c1 }
            // => { a1, b1, c2 }
            // => { a1, b2, c1 }
            // => { a1, b2, c2 }

            // This LINQ uses the Aggregate method to create the Cartesian product of the TUV groups per language.
            // The Aggregate method iterates over the TUV language groups and collects the result set of the lambda
            //  function in the accumulatorCollection. This collection is given an initial seed collection via the first argument.
            IEnumerable<IEnumerable<KeyValuePair<string, TmxNodeCollection>>> seedCollection = new[] { Enumerable.Empty<KeyValuePair<string, TmxNodeCollection>>() };
            var finalCollection = tuvLangGroups.Aggregate(
                seedCollection, 
                (accumulatorCollection, currentInput) =>
                {
                    return from previousItem in accumulatorCollection
                           from currentItem in currentInput
                           select previousItem.Concat(new[] { currentItem });
                });
            
            // Iterate over the the language product and process the TUV element and dump the segment data into the corresponding output files.
            foreach (var tuvLanguageProduct in finalCollection)
            {
                foreach (var tuvLanguageItem in tuvLanguageProduct)
                {
                    this.ProcessTuvTag(tuvLanguageItem.Value, tuvLanguageItem.Key);
                }
            }
        }

        /// <summary>
        /// Processes the TUV element and writes the segment data into the output files.
        /// </summary>
        /// <param name="nodes">The TUV nodes collection.</param>
        /// <param name="language">The language for the TUV element.</param>
        private void ProcessTuvTag(TmxNodeCollection nodes, string language)
        {
            // Extract all <seg> nodes assuming that <seg> nodes are at root.
            TmxNodeCollection segNodes = nodes.FindByName(Token.SEG, false);  

            // Loop through each <seg> node to process the data.
            foreach (TmxNode node in segNodes)
            {
                // Extract current node
                TmxElement currSegNode = (TmxElement)node;
                this.internalCounter = 0;

                this.ProcessSourceSegElement(currSegNode.Nodes, language);
            }

            return;
        }

        /// <summary>
        /// Processes tmx seg nodes.
        /// </summary>
        /// <param name="segNodes">Specifies the html seg nodes.</param>
        /// <param name="language">Specifies the langauge.</param>
        private void ProcessSourceSegElement(TmxNodeCollection segNodes, string language)
        {
            int nodeIndex = -1;
            string text = string.Empty;
            bool flag = false;
            bool isWithinTag = false;
            int[] tags = new int[segNodes.Count];

            foreach (TmxNode node in segNodes)
            {
                nodeIndex++;

                // Check whether current node is processed.
                if (tags[nodeIndex] == 0)
                {
                    if (node is TmxElement)  
                    {
                        TmxElement currNode = (TmxElement)node;

                        // This is non-transferable node
                        if (Str.IsUT(currNode.Name, true))
                        {
                            // Start of new transferable tag
                            if (currNode.Text.Contains(Token.OB))
                            {
                                flag = true;

                                // Get index of node containing }
                                int closingBraceIndex = this.GetClosingTagNodeIndex(segNodes, nodeIndex);

                                // Get index of node containing {
                                int openingBraceIndex = this.GetOpeningTagNodeIndex(segNodes, nodeIndex, closingBraceIndex);

                                // After { of current <tu> there exists more { before closing }
                                if (openingBraceIndex > nodeIndex)
                                {
                                    // Create parent for it.
                                    text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                                    this.internalCounter++;
                                }
                                else
                                {
                                    // Closing ut tag is present
                                    if (closingBraceIndex != -1)
                                    {
                                        // Create parent for it.
                                        text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                                        this.internalCounter++;

                                        // Add the nodes into the parent node of <Ax>.
                                        for (int i = nodeIndex; i <= closingBraceIndex; i++)
                                        {
                                            if (segNodes[i] != null)
                                            {
                                                if (segNodes[i].HTML.Trim().StartsWith("&lt;/", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    isWithinTag = false;
                                                }
                                                else if (segNodes[i].HTML.Trim().StartsWith("&lt;", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    isWithinTag = true;
                                                }
                                            }
                                        }

                                        // Set the closing tag to false.
                                        flag = false;

                                        // Add node index into processed array.
                                        for (int i = nodeIndex; i <= closingBraceIndex; i++)
                                        {
                                            tags[i] = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Create <Ax> parent and add the text to utText string.
                                text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                                this.internalCounter++;
                            }

                            // If current tag has } in its text 
                            if (currNode.Text.Contains(Token.CB))
                            {
                                flag = false;
                            }
                        }
                        else if (Str.IsPH(currNode.Name, true) || Str.IsIT(currNode.Name, true))
                        {
                            text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                            this.internalCounter++;
                        }
                        else if (Str.IsBPT(currNode.Name, true)) 
                        {
                            text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                            this.internalCounter++;
                            isWithinTag = true;
                        }
                        else if (Str.IsEPT(currNode.Name, true))
                        {
                            text = text + Token.ASTART + this.internalCounter.ToString() + Token.EEND;
                            this.internalCounter++;
                            isWithinTag = false;
                        }
                        else
                        {
                            // If there is any other tag inside <seg> tag, we have to treat it as text and dont 
                            // have to process that tag
                            text += currNode.HTML;
                        }
                    }
                    else if (node is TmxText)                           
                    {
                        TmxText currNode = (TmxText)node;

                        // this text is not between { and }
                        if (!flag)       
                        {
                            if (isWithinTag)
                            {
                                text = text + Token.TSTART + 
                                    this.internalCounter + 
                                    Token.GT + 
                                    currNode.HTML +
                                    Token.TEND + 
                                    this.internalCounter +
                                    Token.GT;

                                this.internalCounter++;
                            }
                            else
                            {
                                text = text + currNode.HTML;
                            }
                        }
                    }
                }
            }

            this.writers[language].Write(text);
        }

        /// <summary>
        /// This function returns the index of immediate tag containing '}'.
        /// </summary>
        /// <param name="nodes">Collection of nodes in which } is to be searched.</param>
        /// <param name="nodeIndex">Index of node containig { in its text.</param>
        /// <returns>Index of node containing } in its text.</returns>
        private int GetClosingTagNodeIndex(TmxNodeCollection nodes, int nodeIndex)
        {
            int i = 0;
            for (i = nodeIndex; i < nodes.Count; i++)
            {
                if (nodes[i] is TmxElement)
                {
                    TmxElement currNode = (TmxElement)nodes[i];

                    // This is non-transferable node
                    if (Str.IsUT(currNode.Name, true) && currNode.Text.Contains(Token.CB))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// This function checks whether there is any 'ut' tag containg { between current tag and 
        /// tag with } in its text.
        /// </summary>
        /// <param name="nodes">Collection of nodes in which { is to be searched.</param>
        /// <param name="nodeIndex">Index of node containig { in its text.</param>
        /// <param name="closingTagIndex">Index of closing tag.</param>
        /// <returns>Index of node containing } in its text.</returns>
        private int GetOpeningTagNodeIndex(
            TmxNodeCollection nodes, 
            int nodeIndex, 
            int closingTagIndex)
        {
            int i = 0;

            // Iterate through each tag from current node to last node to find tag containg { in its text.
            for (i = nodeIndex + 1; i <= closingTagIndex; i++)
            {
                if (nodes[i] is TmxElement)
                {
                    TmxElement currNode = (TmxElement)nodes[i];

                    // This is non-transferable node
                    if (Str.IsUT(currNode.Name, true) && currNode.Text.Contains(Token.OB))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
