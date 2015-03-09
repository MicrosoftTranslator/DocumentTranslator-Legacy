using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace MTLWB.HtmlParser
{
    /// <summary>
    /// This is the main HTML parser class. I recommend you don't play around too much in here
    /// as it's a little fiddly.
    /// 
    /// Bascially, this class will build a tree containing HtmlNode elements.
    /// </summary>
    public class HtmlParser
    {
        private static char[] WHITESPACE_CHARS = " \t\r\n".ToCharArray();
        private bool _isRemoveEmptyElementText  = false;
     
        /// <summary>
        /// This constructs a new parser. Even though this object is currently stateless,
        /// in the future, parameters coping for tollerance and SGML (etc.) will be passed.
        /// </summary>
        public HtmlParser()
        {
        }

        /// <summary>
        /// The default mechanism will extract a pure DOM tree, which will contain many text
        /// nodes containing just whitespace (carriage returns etc.) However, with normal
        /// parsing, these are useless and only serve to complicate matters. Therefore, this
        /// option exists to automatically remove those empty text nodes.
        /// </summary>
        /// <returns>Value of _isRemoveEmptyElementText</returns>
        public bool RemoveEmptyElementText
        {
            get
            {
                return _isRemoveEmptyElementText;
            }
            set
            {
                _isRemoveEmptyElementText = value;
            }
        }

        #region The main parser

        /// <summary>
        /// This will parse a string containing HTML and will produce a domain tree.
        /// </summary>
        /// <param name="html">The HTML to be parsed</param>
        /// <returns>A tree representing the elements</returns>
        public HtmlNodeCollection Parse(string html)
        {
            return this.Parse(html, false);
        }

        public HtmlNodeCollection Parse(string html, bool addLineBreaks)
        {
            HtmlNodeCollection nodes = new HtmlNodeCollection(null);

            html = PreprocessScript(html, "script");
            html = PreprocessScript(html, "style");

            html = RemoveComments(html);
            //html = RemoveSGMLComments(html);
            StringCollection tokens = GetTokens(html);

            int index = 0;
            HtmlElement element = null;
            while (index < tokens.Count)
            {
                if ("<".Equals(tokens[index]))
                {
                    // Read open tag

                    index++;
                    if (index >= tokens.Count) break;
                    string tag_name = tokens[index];
                    index++;
                    element = new HtmlElement(tag_name);
                    if (addLineBreaks)
                        element.AddLineBreaks = true;
                    // read the attributes and values

                    while (index < tokens.Count && !">".Equals(tokens[index]) && !"/>".Equals(tokens[index]))
                    {
                        string attribute_name = tokens[index];
                        index++;
                        if (index < tokens.Count && "=".Equals(tokens[index]))
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
                            //HtmlAttribute attribute = new HtmlAttribute(attribute_name, HtmlEncoder.DecodeValue(attribute_value));
                            HtmlAttribute attribute = new HtmlAttribute(attribute_name, attribute_value);
                            element.Attributes.Add(attribute);
                        }
                        else if (index < tokens.Count)
                        {
                            // Null-value attribute
                            HtmlAttribute attribute = new HtmlAttribute(attribute_name, null);
                            element.Attributes.Add(attribute);
                        }
                    }
                    nodes.Add(element);
                    if (index < tokens.Count && "/>".Equals(tokens[index]))
                    {
                        element.IsTerminated = true;
                        index++;
                        element = null;
                    }
                    else if (index < tokens.Count && ">".Equals(tokens[index]))
                    {
                        index++;
                    }
                }
                else if (">".Equals(tokens[index]))
                {
                    index++;
                }
                else if ("</".Equals(tokens[index]))
                {
                    // Read close tag
                    index++;
                    if (index >= tokens.Count) break;
                    string tag_name = tokens[index];
                    index++;

                    int open_index = FindTagOpenNodeIndex(nodes, tag_name);
                    if (open_index != -1)
                    {
                        MoveNodesDown(ref nodes, open_index + 1, (HtmlElement)nodes[open_index]);
                    }
                    else
                    {
                        // Er, there is a close tag without an opening tag!!
                    }

                    // Skip to the end of this tag
                    while (index < tokens.Count && !">".Equals(tokens[index]))
                    {
                        index++;
                    }
                    if (index < tokens.Count && ">".Equals(tokens[index]))
                    {
                        index++;
                    }

                    element = null;
                }
                else
                {
                    // Read text
                    string value = tokens[index];
                    if (_isRemoveEmptyElementText)
                    {
                        value = RemoveWhitespace(value);
                    }                    
                    value = DecodeScript(value);

                    if (_isRemoveEmptyElementText && value.Length == 0)
                    {
                        // We do nothing
                    }
                    else
                    {
                        //if (!(element != null && element.NoEscaping))
                        //{
                        //    value = HtmlEncoder.DecodeValue(value);
                        //}
                        HtmlText node = new HtmlText(value);
                        nodes.Add(node);
                    }
                    index++;
                }
            }
            return nodes;
        }

        /// <summary>
        /// This will move all the nodes from the specified index to the new parent.
        /// </summary>
        /// <param name="nodes">The collection of nodes</param>
        /// <param name="node_index">The index of the first node (in the above collection) to move</param>
        /// <param name="new_parent">The node which will become the parent of the moved nodes</param>
        private void MoveNodesDown(ref HtmlNodeCollection nodes, int node_index, HtmlElement new_parent)
        {
            for (int i = node_index; i < nodes.Count; i++)
            {
                ((HtmlElement)new_parent).Nodes.Add(nodes[i]);
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
        /// <param name="nodes">The collection of nodes</param>
        /// <param name="name">The name of the tag</param>
        /// <returns>The index of the opening tag, or -1 if it was not found</returns>
        private int FindTagOpenNodeIndex(HtmlNodeCollection nodes, string name)
        {
            for (int index = nodes.Count - 1; index >= 0; index--)
            {
                if (nodes[index] is HtmlElement)
                {
                    if (((HtmlElement)nodes[index]).Name.ToLower().Equals(name.ToLower()) && ((HtmlElement)nodes[index]).Nodes.Count == 0 && ((HtmlElement)nodes[index]).IsTerminated == false)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region HTML clean-up functions

        /// <summary>
        /// This will remove redundant whitespace from the string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveWhitespace(string input)
        {
            string output = input.Replace("\r", "");
            output = output.Replace("\n", "");
            output = output.Replace("\t", " ");
            if (output.Trim().Length == 0)
            {
                output = output.Trim();
            }
            //output = output.Trim();
            return output;
        }

        /// <summary>
        /// This will remove all HTML comments from the input string. This will
        /// not remove comment markers from inside tag attribute values.
        /// </summary>
        /// <param name="input">Input HTML containing comments</param>
        /// <returns>HTML containing no comments</returns>
        private string RemoveComments(string input)
        {
            StringBuilder output = new StringBuilder();

            int i = 0;
            bool inTag = false;

            while (i < input.Length)
            {
                if (i + 4 < input.Length && input.Substring(i, 4).Equals("<!--"))
                {
                    i += 4;
                    i = input.IndexOf("-->", i);
                    if (i == -1)
                    {
                        break;
                    }
                    i += 3;
                }
                else if (input.Substring(i, 1).Equals("<"))
                {
                    inTag = true;
                    output.Append("<");
                    i++;
                }
                else if (input.Substring(i, 1).Equals(">"))
                {
                    inTag = false;
                    output.Append(">");
                    i++;
                }
                else if (input.Substring(i, 1).Equals("\"") && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf("\"", i);
                    if (i == -1)
                    {
                        //break; //Commented by Tejas on Oct 13, 09
                        i = input.Length - 1;
                    }
                    i++;
                    output.Append(input.Substring(string_start, i - string_start));
                }
                else if (input.Substring(i, 1).Equals("\'") && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf("\'", i);
                    if (i == -1)
                    {
                        //break; //Commented by Tejas on Oct 13, 09
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

        /// <summary>
        /// This will remove all HTML comments from the input string. This will
        /// not remove comment markers from inside tag attribute values.
        /// </summary>
        /// <param name="input">Input HTML containing comments</param>
        /// <returns>HTML containing no comments</returns>
        private string RemoveSGMLComments(string input)
        {
            StringBuilder output = new StringBuilder();

            int i = 0;
            bool inTag = false;

            while (i < input.Length)
            {
                if (i + 2 < input.Length && input.Substring(i, 2).Equals("<!"))
                {
                    i += 2;
                    i = input.IndexOf(">", i);
                    if (i == -1)
                    {
                        break;
                    }
                    i += 3;
                }
                else if (input.Substring(i, 1).Equals("<"))
                {
                    inTag = true;
                    output.Append("<");
                    i++;
                }
                else if (input.Substring(i, 1).Equals(">"))
                {
                    inTag = false;
                    output.Append(">");
                    i++;
                }
                else if (input.Substring(i, 1).Equals("\"") && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf("\"", i);
                    if (i == -1)
                    {
                        break;
                    }
                    i++;
                    output.Append(input.Substring(string_start, i - string_start));
                }
                else if (input.Substring(i, 1).Equals("\'") && inTag)
                {
                    int string_start = i;
                    i++;
                    i = input.IndexOf("\'", i);
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

        /// <summary>
        /// This will encode the scripts within the page so they get passed through the
        /// parser properly. This is due to some people using comments protect the script
        /// and others who don't. It also takes care of issues where the script itself has
        /// HTML comments in (in strings, for example).
        /// </summary>
        /// <param name="input">The HTML to examine</param>
        /// <returns>The HTML with the scripts marked up differently</returns>
        private string PreprocessScript(string input, string tag_name)
        {
            StringBuilder output = new StringBuilder();
            int index = 0;
            int tag_name_len = tag_name.Length;
            while (index < input.Length)
            {
                bool omit_body = false;
                if (index + tag_name_len + 1 < input.Length && input.Substring(index, tag_name_len + 1).ToLower().Equals("<" + tag_name))
                {
                    // Look for the end of the tag (we pass the attributes through as normal)
                    do
                    {
                        if (index >= input.Length)
                        {
                            break;
                        }
                        else if (input.Substring(index, 1).Equals(">"))
                        {
                            output.Append(">");
                            index++;
                            break;
                        }
                        else if (index + 1 < input.Length && input.Substring(index, 2).Equals("/>"))
                        {
                            output.Append("/>");
                            index += 2;
                            omit_body = true;
                            break;
                        }
                        else if (input.Substring(index, 1).Equals("\""))
                        {
                            output.Append("\"");
                            index++;
                            while (index < input.Length && !input.Substring(index, 1).Equals("\""))
                            {
                                output.Append(input.Substring(index, 1));
                                index++;
                            }
                            if (index < input.Length)
                            {
                                index++;
                                output.Append("\"");
                            }
                        }
                        else if (input.Substring(index, 1).Equals("\'"))
                        {
                            output.Append("\'");
                            index++;
                            while (index < input.Length && !input.Substring(index, 1).Equals("\'"))
                            {
                                output.Append(input.Substring(index, 1));
                                index++;
                            }
                            if (index < input.Length)
                            {
                                index++;
                                output.Append("\'");
                            }
                        }
                        else
                        {
                            output.Append(input.Substring(index, 1));
                            index++;
                        }
                    } while (true);
                    if (index >= input.Length) break;
                    // Phew! Ok now we are reading the script body

                    if (!omit_body)
                    {
                        StringBuilder script_body = new StringBuilder();
                        while (index + tag_name_len + 3 < input.Length && !input.Substring(index, tag_name_len + 3).ToLower().Equals("</" + tag_name + ">"))
                        {
                            script_body.Append(input.Substring(index, 1));
                            index++;
                        }
                        // Done - now encode the script
                        output.Append(EncodeScript(script_body.ToString()));
                        output.Append("</" + tag_name + ">");
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

        /// <summary>
        /// This function will encode the script
        /// </summary>
        /// <param name="script">Script to encode</param>
        /// <returns>Encoded script</returns>
        private static string EncodeScript(string script)
        {
            string output = script.Replace("<", "[MTLWB-SCRIPT-LT]");
            output = output.Replace(">", "[MTLWB-SCRIPT-GT]");
            output = output.Replace("\r", "[MTLWB-SCRIPT-CR]");
            output = output.Replace("\n", "[MTLWB-SCRIPT-LF]");
            return output;
        }

        /// <summary>
        /// this function will decode the script
        /// </summary>
        /// <param name="script">Script to decode</param>
        /// <returns>Decodes script</returns>
        private static string DecodeScript(string script)
        {
            string output = script.Replace("[MTLWB-SCRIPT-LT]", "<");
            output = output.Replace("[MTLWB-SCRIPT-GT]", ">");
            output = output.Replace("[MTLWB-SCRIPT-CR]", "\r");
            output = output.Replace("[MTLWB-SCRIPT-LF]", "\n");
            return output;
        }

        #endregion

        #region HTML tokeniser

        /// <summary>
        /// This will tokenise the HTML input string.
        /// </summary>
        /// <param name="input">String to tokenise</param>
        /// <returns>Tokens</returns>
        public StringCollection GetTokens(string input)
        {
            StringCollection tokens = new StringCollection();

            int i = 0;
            ParseStatus status = ParseStatus.ReadText;

            while (i < input.Length)
            {
                if (status == ParseStatus.ReadText)
                {
                    if (i + 2 < input.Length && input.Substring(i, 2).Equals("</"))
                    {
                        i += 2;
                        tokens.Add("</");
                        status = ParseStatus.ReadEndTag;
                    }
                    else if (input.Substring(i, 1).Equals("<"))
                    {
                        i++;
                        tokens.Add("<");
                        status = ParseStatus.ReadStartTag;
                    }
                    else
                    {
                        int next_index = input.IndexOf("<", i);
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
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    // Read tag name
                    int tag_name_start = i;

                    if (i < input.Length && input.Substring(i, 1).Equals("!"))
                    {
                        while (i < input.Length && !input.Substring(i, 1).Equals(">"))
                        {
                            i++;
                        }
                        tokens.Add(input.Substring(tag_name_start, i - tag_name_start));
                        tokens.Add(">");
                        i++;
                        status = ParseStatus.ReadText;
                        continue;
                    }

                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(" \r\n\t/>".ToCharArray()) == -1)
                    {
                        i++;
                    }
                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start));
                    // Skip trailing whitespace in tag
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    if (i + 1 < input.Length && input.Substring(i, 2).Equals("/>"))
                    {
                        tokens.Add("/>");
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals(">"))
                    {
                        tokens.Add(">");
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
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    // Read tag name
                    int tag_name_start = i;
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(" \r\n\t>".ToCharArray()) == -1)
                    {
                        i++;
                    }
                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start));
                    // Skip trailing whitespace in tag
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    if (i < input.Length && input.Substring(i, 1).Equals(">"))
                    {
                        tokens.Add(">");
                        status = ParseStatus.ReadText;
                        i++;
                    }
                }
                else if (status == ParseStatus.ReadAttributeName)
                {
                    // Read attribute name
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    int attribute_name_start = i;
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(" \r\n\t/>=".ToCharArray()) == -1)
                    {
                        i++;
                    }
                    tokens.Add(input.Substring(attribute_name_start, i - attribute_name_start));
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    if (i + 1 < input.Length && input.Substring(i, 2).Equals("/>"))
                    {
                        tokens.Add("/>");
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals(">"))
                    {
                        tokens.Add(">");
                        status = ParseStatus.ReadText;
                        i++;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals("="))
                    {
                        tokens.Add("=");
                        i++;
                        status = ParseStatus.ReadAttributeValue;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals("/"))
                    {
                        i++;
                    }
                }
                else if (status == ParseStatus.ReadAttributeValue)
                {
                    // Read the attribute value
                    while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                    {
                        i++;
                    }
                    if (i < input.Length && input.Substring(i, 1).Equals("\""))
                    {
                        int value_start = i;
                        i++;
                        while (i < input.Length && !input.Substring(i, 1).Equals("\""))
                        {
                            i++;
                        }
                        if (i < input.Length && input.Substring(i, 1).Equals("\""))
                        {
                            i++;
                        }
                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2));
                        status = ParseStatus.ReadAttributeName;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals("\'"))
                    {
                        int value_start = i;
                        i++;
                        while (i < input.Length && !input.Substring(i, 1).Equals("\'"))
                        {
                            i++;
                        }
                        if (i < input.Length && input.Substring(i, 1).Equals("\'"))
                        {
                            i++;
                        }
                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2));
                        status = ParseStatus.ReadAttributeName;
                    }
                    else
                    {
                        int value_start = i;
                        while (i < input.Length && input.Substring(i, 1).IndexOfAny(" \r\n\t/>".ToCharArray()) == -1)
                        {
                            i++;
                        }
                        tokens.Add(input.Substring(value_start, i - value_start));
                        while (i < input.Length && input.Substring(i, 1).IndexOfAny(WHITESPACE_CHARS) != -1)
                        {
                            i++;
                        }
                        status = ParseStatus.ReadAttributeName;
                    }
                    if (i + 1 < input.Length && input.Substring(i, 2).Equals("/>"))
                    {
                        tokens.Add("/>");
                        status = ParseStatus.ReadText;
                        i += 2;
                    }
                    else if (i < input.Length && input.Substring(i, 1).Equals(">"))
                    {
                        tokens.Add(">");
                        i++;
                        status = ParseStatus.ReadText;
                    }
                    // ANDY
                }
            }

            return tokens;
        }

        #endregion

        /// <summary>
        /// Internal FSM to represent the state of the parser
        /// </summary>
        private enum ParseStatus
        {
            ReadText = 0,
            ReadEndTag = 1,
            ReadStartTag = 2,
            ReadAttributeName = 3,
            ReadAttributeValue = 4
        };
    }
}
