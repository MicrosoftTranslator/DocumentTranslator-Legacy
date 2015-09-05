// -
// <copyright file="HtmlParser.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System.Collections.Specialized;
using System.Text;

namespace Mts.Common.Tmx.Parser
{
    /// <summary>
    /// This is the main HTML parser class. I recommend you don't play around too much in here
    /// as it's a little fiddly.
    /// Bascially, this class will build a tree containing HtmlNode elements.
    /// </summary>
    public class TmxParser
    {
        private bool isRemoveEmptyElementText;

        /// <summary>
        /// Initializes a new instance of the TmxParser class. This constructs a new parser. 
        /// Even though this object is currently stateless, in the future, parameters coping 
        /// for tollerance and SGML (etc.) will be passed.
        /// </summary>
        public TmxParser()
        {
            this.isRemoveEmptyElementText = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to remove empty element text.
        /// The default mechanism will extract a pure DOM tree, which will contain many text
        /// nodes containing just whitespace (carriage returns etc.) However, with normal
        /// parsing, these are useless and only serve to complicate matters. Therefore, this
        /// option exists to automatically remove those empty text nodes.
        /// </summary>
        public bool RemoveEmptyElementText
        {
            get
            {
                return this.isRemoveEmptyElementText;
            }

            set
            {
                this.isRemoveEmptyElementText = value;
            }
        }

        /*
         * This method contains valid code. Commented for code coverage purpose.
         * 
        /// <summary>
        /// This will parse a string containing HTML and will produce a domain tree.
        /// </summary>
        /// <param name="html">The HTML to be parsed.</param>
        /// <returns>A tree representing the elements.</returns>
        public TmxNodeCollection Parse(string html)
        {
            return this.Parse(html, false);
        }
        */

        public TmxNodeCollection Parse(string html, bool addLineBreaks)
        {
            TmxNodeCollection nodes = new TmxNodeCollection(null);

            html = this.PreprocessScript(html, Token.SCRIPT);
            html = this.PreprocessScript(html, Token.STYLE);

            html = this.RemoveComments(html);

            StringCollection tokens = this.GetTokens(html);

            int index = 0;
            TmxElement element = null;
            while (index < tokens.Count)
            {
                if (Str.IsLT(tokens[index]))
                {
                    // Read open tag
                    index++;
                    if (index >= tokens.Count)
                    {
                        break;
                    }

                    string tag_name = tokens[index];
                    index++;
                    element = new TmxElement(tag_name);

                    if (addLineBreaks)
                    {
                        element.AddLineBreaks = true;
                    }

                    // Read the attributes and values.
                    while (index < tokens.Count && !Str.IsGT(tokens[index]) && !Str.IsSLGT(tokens[index]))
                    {
                        string attribute_name = tokens[index];
                        index++;

                        if (index < tokens.Count && Str.IsEQ(tokens[index]))
                        {
                            index++;
                            string attribute_value;
                            if (index < tokens.Count)
                            {
                                attribute_value = tokens[index];
                            }
                            else
                            {
                                attribute_value = null;
                            }

                            index++;

                            TmxAttribute attribute = new TmxAttribute(attribute_name, attribute_value);
                            element.Attributes.Add(attribute);
                        }
                        else if (index < tokens.Count)
                        {
                            // Null-value attribute
                            TmxAttribute attribute = new TmxAttribute(attribute_name, null);
                            element.Attributes.Add(attribute);
                        }
                    }

                    nodes.Add(element);
                    if (index < tokens.Count && Str.IsSLGT(tokens[index]))
                    {
                        element.IsTerminated = true;
                        index++;
                        element = null;
                    }
                    else if (index < tokens.Count && Str.IsGT(tokens[index]))
                    {
                        index++;
                    }
                }
                else if (Str.IsGT(tokens[index]))
                {
                    index++;
                }
                else if (Str.IsLTSL(tokens[index]))
                {
                    // Read close tag
                    index++;
                    if (index >= tokens.Count)
                    {
                        break;
                    }

                    string tag_name = tokens[index];
                    index++;

                    int open_index = this.FindTagOpenNodeIndex(nodes, tag_name);
                    if (open_index != -1)
                    {
                        this.MoveNodesDown(ref nodes, open_index + 1, (TmxElement)nodes[open_index]);
                    }
                    else
                    {
                        // Er, there is a close tag without an opening tag!!
                    }

                    // Skip to the end of this tag
                    while (index < tokens.Count && !Str.IsGT(tokens[index]))
                    {
                        index++;
                    }

                    if (index < tokens.Count && Str.IsGT(tokens[index]))
                    {
                        index++;
                    }

                    element = null;
                }
                else
                {
                    // Read text
                    string value = tokens[index];

                    if (this.isRemoveEmptyElementText)
                    {
                        value = this.RemoveWhitespace(value);
                    }

                    value = DecodeScript(value);

                    if (this.isRemoveEmptyElementText && value.Length == 0)
                    {
                        // We do nothing
                    }
                    else
                    {
                        TmxText node = new TmxText(value);
                        nodes.Add(node);
                    }

                    index++;
                }
            }

            return nodes;
        }

        /// <summary>
        /// This will tokenise the HTML input string.
        /// </summary>
        /// <param name="input">String to tokenise.</param>
        /// <returns>Returns tokens.</returns>
        public StringCollection GetTokens(string input)
        {
            StringCollection tokens = new StringCollection();

            int i = 0;
            ParseStatus status = ParseStatus.ReadText;

            while (i < input.Length)
            {
                if (status == ParseStatus.ReadText)
                {
                    if (i + 2 < input.Length && Str.IsLTSL(input, i, 2))
                    {
                        i += 2;
                        tokens.Add(Token.LTSL);
                        status = ParseStatus.ReadEndTag;
                    }
                    else if (Str.IsLT(input, i, 1))
                    {
                        i++;
                        tokens.Add(Token.LT);
                        status = ParseStatus.ReadStartTag;
                    }
                    else
                    {
                        int next_index = input.IndexOf(Token.LT, i);
                        if (next_index == -1)
                        {
                            tokens.Add(input.Substring(i));
                            break;
                        }
                        else
                        {
                            tokens.Add(input.Substring(i, next_index - i));
                            i = next_index;
                        }
                    }
                }
                else if (status == ParseStatus.ReadStartTag)
                {
                    // Skip leading whitespace in tag
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    // Read tag name
                    int tag_name_start = i;

                    if (i < input.Length && Str.IsEX(input, i, 1))
                    {
                        while (i < input.Length && !Str.IsGT(input, i, 1))
                        {
                            i++;
                        }

                        tokens.Add(input.Substring(tag_name_start, i - tag_name_start));
                        tokens.Add(Token.GT);
                        i++;
                        status = ParseStatus.ReadText;
                        continue;
                    }

                    while (i < input.Length && !Str.HasWhiteSpaceChars1(input, i, 1))
                    {
                        i++;
                    }

                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start));

                    // Skip trailing whitespace in tag
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    if (i + 1 < input.Length && Str.IsSLGT(input, i, 2))
                    {
                        tokens.Add(Token.SLGT);
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && Str.IsGT(input, i, 1))
                    {
                        tokens.Add(Token.GT);
                        status = ParseStatus.ReadText;
                        i++;
                    }
                    else
                    {
                        status = ParseStatus.ReadAttributeName;
                    }
                }
                else if (status == ParseStatus.ReadEndTag)
                {
                    // Skip leading whitespace in tag
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    // Read tag name
                    int tag_name_start = i;

                    while (i < input.Length && !Str.HasWhiteSpaceChars1(input, i, 1))
                    {
                        i++;
                    }

                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start));

                    // Skip trailing whitespace in tag
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    if (i < input.Length && Str.IsGT(input, i, 1))
                    {
                        tokens.Add(Token.GT);
                        status = ParseStatus.ReadText;
                        i++;
                    }
                }
                else if (status == ParseStatus.ReadAttributeName)
                {
                    // Read attribute name
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    int attribute_name_start = i;

                    while (i < input.Length && !Str.HasWhiteSpaceChars2(input, i, 1))
                    {
                        i++;
                    }

                    tokens.Add(input.Substring(attribute_name_start, i - attribute_name_start));

                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    if (i + 1 < input.Length && Str.IsSLGT(input, i, 2))
                    {
                        tokens.Add(Token.SLGT);
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && Str.IsGT(input, i, 1))
                    {
                        tokens.Add(Token.GT);
                        status = ParseStatus.ReadText;
                        i++;
                    }
                    else if (i < input.Length && Str.IsEQ(input, i, 1))
                    {
                        tokens.Add(Token.EQ);
                        i++;
                        status = ParseStatus.ReadAttributeValue;
                    }
                    else if (i < input.Length && Str.IsSL(input, i, 1))
                    {
                        i++;
                    }
                }
                else if (status == ParseStatus.ReadAttributeValue)
                {
                    // Read the attribute value
                    while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                    {
                        i++;
                    }

                    if (i < input.Length && Str.IsDQ(input, i, 1))
                    {
                        int value_start = i;
                        i++;

                        while (i < input.Length && !Str.IsDQ(input, i, 1))
                        {
                            i++;
                        }

                        if (i < input.Length && Str.IsDQ(input, i, 1))
                        {
                            i++;
                        }

                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2));
                        status = ParseStatus.ReadAttributeName;
                    }
                    else if (i < input.Length && Str.IsSQ(input, i, 1))
                    {
                        int value_start = i;
                        i++;

                        while (i < input.Length && !Str.IsSQ(input, i, 1))
                        {
                            i++;
                        }

                        if (i < input.Length && Str.IsSQ(input, i, 1))
                        {
                            i++;
                        }

                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2));
                        status = ParseStatus.ReadAttributeName;
                    }
                    else
                    {
                        int value_start = i;
                        while (i < input.Length && !Str.HasWhiteSpaceChars1(input, i, 1))
                        {
                            i++;
                        }

                        tokens.Add(input.Substring(value_start, i - value_start));
                        while (i < input.Length && Str.HasWhiteSpaceChars(input, i, 1))
                        {
                            i++;
                        }

                        status = ParseStatus.ReadAttributeName;
                    }

                    if (i + 1 < input.Length && Str.IsSLGT(input, i, 2))
                    {
                        tokens.Add(Token.SLGT);
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && Str.IsGT(input, i, 1))
                    {
                        tokens.Add(Token.GT);
                        i++;
                        status = ParseStatus.ReadText;
                    }
                }
            }

            return tokens;
        }

        /// <summary>
        /// This function will encode the script.
        /// </summary>
        /// <param name="script">Script to encode.</param>
        /// <returns>Encoded script.</returns>
        private static string EncodeScript(string script)
        {
            string output = script.Replace(Token.LT, Token.MTLWBSCRIPTLT);
            output = output.Replace(Token.GT, Token.MTLWBSCRIPTGT);
            output = output.Replace(Token.CR, Token.MTLWBSCRIPTCR);
            output = output.Replace(Token.NL, Token.MTLWBSCRIPTLF);
            return output;
        }

        /// <summary>
        /// This function will decode the script.
        /// </summary>
        /// <param name="script">Script to decode.</param>
        /// <returns>Decodes script.</returns>
        private static string DecodeScript(string script)
        {
            string output = script.Replace(Token.MTLWBSCRIPTLT, Token.LT);
            output = output.Replace(Token.MTLWBSCRIPTGT, Token.GT);
            output = output.Replace(Token.MTLWBSCRIPTCR, Token.CR);
            output = output.Replace(Token.MTLWBSCRIPTLF, Token.NL);
            return output;
        }

        /// <summary>
        /// This will move all the nodes from the specified index to the new parent.
        /// </summary>
        /// <param name="nodes">The collection of nodes.</param>
        /// <param name="node_index">The index of the first node (in the above collection) to move.</param>
        /// <param name="new_parent">The node which will become the parent of the moved nodes.</param>
        private void MoveNodesDown(ref TmxNodeCollection nodes, int node_index, TmxElement new_parent)
        {
            for (int i = node_index; i < nodes.Count; i++)
            {
                ((TmxElement)new_parent).Nodes.Add(nodes[i]);
                nodes[i].SetParent(new_parent);
            }

            int c = nodes.Count;

            for (int i = node_index; i < c; i++)
            {
                nodes.RemoveAt(node_index);
            }

            new_parent.IsExplicitlyTerminated = true;
        }

        /// <summary>
        /// This will find the corresponding opening tag for the named one. This is identified as
        /// the most recently read node with the same name, but with no child nodes.
        /// </summary>
        /// <param name="nodes">The collection of nodes.</param>
        /// <param name="name">The name of the tag.</param>
        /// <returns>The index of the opening tag, or -1 if it was not found.</returns>
        private int FindTagOpenNodeIndex(TmxNodeCollection nodes, string name)
        {
            for (int index = nodes.Count - 1; index >= 0; index--)
            {
                if (nodes[index] is TmxElement)
                {
                    if (Str.IsEqual(((TmxElement)nodes[index]).Name, name, true) &&
                       ((TmxElement)nodes[index]).Nodes.Count == 0 &&
                       ((TmxElement)nodes[index]).IsTerminated == false)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes redundant whitespace from the string.
        /// </summary>
        /// <param name="input">Specifies input string.</param>
        /// <returns>Return a whitespace removed string.</returns>
        private string RemoveWhitespace(string input)
        {
            string output = input.Replace(Token.CR, string.Empty);
            output = output.Replace(Token.NL, string.Empty);
            output = output.Replace(Token.TAB, string.Empty);

            if (output.Trim().Length == 0)
            {
                output = output.Trim();
            }

            return output;
        }

        /// <summary>
        /// This will remove all HTML comments from the input string. This will
        /// not remove comment markers from inside tag attribute values.
        /// </summary>
        /// <param name="input">Input HTML containing comments.</param>
        /// <returns>HTML containing no comments.</returns>
        private string RemoveComments(string input)
        {
            StringBuilder output = new StringBuilder();

            int i = 0;
            bool inTag = false;

            while (i < input.Length)
            {
                if (i + 4 < input.Length && Str.IsXCS(input, i, 4))
                {
                    i += 4;
                    i = input.IndexOf(Token.XCE, i);

                    if (i == -1)
                    {
                        break;
                    }

                    i += 3;
                }
                else if (Str.IsLT(input, i, 1))
                {
                    inTag = true;
                    output.Append(Token.LT);
                    i++;
                }
                else if (Str.IsGT(input, i, 1))
                {
                    inTag = false;
                    output.Append(Token.GT);
                    i++;
                }
                else if (Str.IsDQ(input, i, 1) && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf(Token.DQ, i);

                    if (i == -1)
                    {
                        i = input.Length - 1;
                    }

                    i++;

                    output.Append(input.Substring(string_start, i - string_start));
                }
                else if (Str.IsSQ(input, i, 1) && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf(Token.SQ, i);

                    if (i == -1)
                    {
                        i = input.Length - 1;
                    }

                    i++;
                    output.Append(input.Substring(string_start, i - string_start));
                }
                else
                {
                    output.Append(input.Substring(i, 1));
                    i++;
                }
            }

            return output.ToString();
        }

        /*
         *  
         * This Method contains valid code.
         * Commented for code coverage purpose.
         * 
        /// <summary>
        /// This will remove all HTML comments from the input string. This will
        /// not remove comment markers from inside tag attribute values.
        /// </summary>
        /// <param name="input">Input HTML containing comments.</param>
        /// <returns>HTML containing no comments.</returns>
        private string RemoveSGMLComments(string input)
        {
            StringBuilder output = new StringBuilder();

            int i = 0;
            bool inTag = false;

            while (i < input.Length)
            {
                if (i + 2 < input.Length && Str.IsSGMLCS(input, i, 2))
                {
                    i += 2;
                    i = input.IndexOf(Token.GT, i);

                    if (i == -1)
                    {
                        break;
                    }

                    i += 3;
                }
                else if (Str.IsLT(input, i, 1))
                {
                    inTag = true;
                    output.Append(Token.LT);
                    i++;
                }
                else if (Str.IsGT(input, i, 1))
                {
                    inTag = false;
                    output.Append(Token.GT);
                    i++;
                }
                else if (Str.IsDQ(input, i, 1) && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf(Token.DQ, i);
                    if (i == -1)
                    {
                        break;
                    }

                    i++;
                    output.Append(input.Substring(string_start, i - string_start));
                }
                else if (Str.IsSQ(input, i, 1) && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf(Token.SQ, i);
                    if (i == -1)
                    {
                        break;
                    }

                    i++;
                    output.Append(input.Substring(string_start, i - string_start));
                }
                else
                {
                    output.Append(input.Substring(i, 1));
                    i++;
                }
            }

            return output.ToString();
        }
         */

        /// <summary>
        /// This will encode the scripts within the page so they get passed through the
        /// parser properly. This is due to some people using comments protect the script
        /// and others who don't. It also takes care of issues where the script itself has
        /// HTML comments in (in strings, for example).
        /// </summary>
        /// <param name="input">The HTML to examine.</param>
        /// <returns>The HTML with the scripts marked up differently.</returns>
        private string PreprocessScript(string input, string tag_name)
        {
            StringBuilder output = new StringBuilder();
            int index = 0;
            int tag_name_len = tag_name.Length;

            while (index < input.Length)
            {
                bool omit_body = false;

                if (index + tag_name_len + 1 < input.Length && Str.IsSubStrEqual(input, index, tag_name_len + 1, Token.LT + tag_name, true))
                {
                    // Look for the end of the tag (we pass the attributes through as normal)
                    do
                    {
                        if (index >= input.Length)
                        {
                            break;
                        }
                        else if (Str.IsGT(input, index, 1))
                        {
                            output.Append(Token.GT);
                            index++;
                            break;
                        }
                        else if (index + 1 < input.Length && Str.IsSLGT(input, index, 2))
                        {
                            output.Append(Token.SLGT);
                            index += 2;
                            omit_body = true;
                            break;
                        }
                        else if (Str.IsDQ(input, index, 1))
                        {
                            output.Append(Token.DQ);
                            index++;
                            while (index < input.Length && !Str.IsDQ(input, index, 1))
                            {
                                output.Append(input.Substring(index, 1));
                                index++;
                            }

                            if (index < input.Length)
                            {
                                index++;
                                output.Append(Token.DQ);
                            }
                        }
                        else if (Str.IsSQ(input, index, 1))
                        {
                            output.Append(Token.SQ);
                            index++;
                            while (index < input.Length && !Str.IsSQ(input, index, 1))
                            {
                                output.Append(input.Substring(index, 1));
                                index++;
                            }

                            if (index < input.Length)
                            {
                                index++;
                                output.Append(Token.SQ);
                            }
                        }
                        else
                        {
                            output.Append(input.Substring(index, 1));
                            index++;
                        }
                    }
                    while (true);

                    if (index >= input.Length)
                    {
                        break;
                    }

                    // Phew! Ok now we are reading the script body
                    if (!omit_body)
                    {
                        StringBuilder script_body = new StringBuilder();
                        while (index + tag_name_len + 3 < input.Length && !Str.IsSubStrEqual(input, index, tag_name_len + 3, Token.LTSL + tag_name + Token.GT, true))
                        {
                            script_body.Append(input.Substring(index, 1));
                            index++;
                        }

                        // Done - now encode the script
                        output.Append(EncodeScript(script_body.ToString()));
                        output.Append(Token.LTSL + tag_name + Token.GT);
                        if (index + tag_name_len + 3 < input.Length)
                        {
                            index += tag_name_len + 3;
                        }
                    }
                }
                else
                {
                    output.Append(input.Substring(index, 1));
                    index++;
                }
            }

            return output.ToString();
        }
    }
}
