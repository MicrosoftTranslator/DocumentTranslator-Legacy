
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace MTLWB.Common.IO
{
    /// <summary>
    /// Enum to define the type of tag
    /// </summary>
    public enum TagType
    {
        External,
        Internal,
        InternalGroup,
        ExternalGroup,
        NotDefined
    }


    /// <summary>
    /// Enum to define the type of attribute
    /// </summary>
    public enum AttributeType
    {
        Translatable,
        NonTranslatable
    }

    /// <summary>
    /// Enum to define the type of INI file
    /// </summary>
    public enum IniFileType
    {
        Trados,
        NonTranslatableElements,
        TranslatableElements,
    }

    /// <summary>
    /// Parses the INI file and provides fuctions to read various sections of INI file.
    /// </summary>
    public class IniFileReader
    {
        private const int MAX_SIZE = 65535;
        private const int INITIAL_BUFFER_SIZE = 1024;
        private Dictionary<string, TagType> _IniTagsKeyValue;
        private List<string> _IniAttributeValue;    // Holds attributes

        #region Import Win32 API

        /// <summary>
        /// Retrieves values for the specified key and section of an Ini file
        /// </summary>        
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// Retrieves all the keys and values for the specified section of an Ini file
        /// </summary>
        [DllImport("kernel32.dll")]
        private extern static int GetPrivateProfileSection(
            string section, [MarshalAs(UnmanagedType.LPArray)] byte[] returnedString,
            int size, string filePath);

        #endregion

        /// <summary>
        /// Constructor taking INI file path as parameter
        /// </summary>
        /// <param name="filePath">Physical path of INI file</param>
        public IniFileReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            FilePath = filePath;
            FileType = GetIniFileType();
            IsCaseSensitive = false;
        }

        /// <summary>
        /// Gets or sets Ini file path to be read.
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets whether the INI file is to be treated as case sensitive or not.
        /// </summary>
        public bool IsCaseSensitive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the type of INI file
        /// </summary>
        public IniFileType FileType
        {
            get;
            private set;
        }

        /// <summary>
        /// Reads and returns the value of DescriptiveName from "GeneralSGMLSettings" section of the Ini file
        /// </summary>
        /// <returns>The value of DescriptiveName in INI file</returns>
        public string GetDescriptiveName()
        {
            return IniReadValue("GeneralSGMLSettings", "DescriptiveName").Trim();
        }

        /// <summary>
        /// Reads and returns the value of DOCTYPE from "GeneralSGMLSettings" section of the Ini file
        /// </summary>
        /// <returns>The value of DOCTYPE in INI file</returns>
        public string GetRootElementName()
        {
            return IniReadValue("GeneralSGMLSettings", "DOCTYPE").Trim();
        }

        /// <summary>
        /// Reads and returns the the list of space separated values of DOCTYPE from "GeneralSGMLSettings" section of the Ini file (To support multiple root elemetns in INI)
        /// </summary>
        /// <returns>The list of space separated values from DOCTYPE in INI file</returns>
        public List<string> GetRootElementNames()
        {
            List<string> rootElements = new List<string>();
            string RootElementNames = string.Empty;
            RootElementNames = IniReadValue("GeneralSGMLSettings", "DOCTYPE");
            IEnumerator enumStr = RootElementNames.Split(' ').GetEnumerator();
            while (enumStr.MoveNext())
            {
                if (enumStr.Current.ToString().Trim() != "")
                    rootElements.Add(enumStr.Current.ToString().Trim());
            }
            return rootElements;
        }

        /// <summary>
        /// Gets the boolean value if the given tagtype is present in an array
        /// </summary>
        /// <param name="splittedItems">Contains Ini "Tags" section in Array</param>
        /// <param name="TagType">Indicataes the tag which has to be varified in array</param>
        /// <returns>True if the match found</returns>
        public bool CheckTagType(string[] splittedItems, string TagType)
        {
            for (int i = 3; i < splittedItems.Length; i++)
            {
                if (splittedItems[i].Equals(TagType, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the TagType of the given tag name
        /// </summary>
        /// <param name="grandParent">Name of the grand parent element of currnet text node</param>
        /// <param name="parent">Name of the parent element of currnet text node</param>
        /// <returns>TagType of the Tag in Ini file</returns>
        public TagType GetTagType(string grandParent, string parent)
        {
            if (string.IsNullOrEmpty(grandParent))
                throw new ArgumentNullException("grandParent");
            if (string.IsNullOrEmpty(parent))
                throw new ArgumentNullException("parent");

            if (_IniTagsKeyValue == null)
            {
                LoadIniTagSection();
            }
            if (!IsCaseSensitive)
            {
                parent = parent.ToLower();
                grandParent = grandParent.ToLower();
            }
            string tagName = grandParent + "/" + parent;
            if (_IniTagsKeyValue.ContainsKey(tagName))
            {
                return _IniTagsKeyValue[tagName];
            }
            else if (_IniTagsKeyValue.ContainsKey(parent))
            {
                return _IniTagsKeyValue[parent];
            }

            //If tag name is not found in Ini, return default tag type as per defaultTagStyle in Ini file
            TagType defaultTagType = GetDefaultTagStyle();
            return defaultTagType;
        }

        /// <summary>
        /// Gets the TagType of the given tag name
        /// </summary>
        /// <param name="tagName">Name of the tag in Ini file</param>
        /// <returns>TagType of the Tag in Ini file</returns>
        public TagType GetTagType(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentNullException("tagName");

            if (tagName.Equals("meta", StringComparison.InvariantCultureIgnoreCase)) 
                return TagType.NotDefined;

            if (_IniTagsKeyValue == null)
            {
                LoadIniTagSection();
            }
            if (!IsCaseSensitive)
            {
                tagName = tagName.ToLower();
            }
            if (_IniTagsKeyValue.ContainsKey(tagName))
            {
                return _IniTagsKeyValue[tagName];
            }

            //If tag name is not found in Ini, return default tag type as per defaultTagStyle in Ini file
            TagType defaultTagType = GetDefaultTagStyle();
            return defaultTagType;
        }

        /// <summary>
        /// This method will read the given key value from the given Ini Section
        /// </summary>
        /// <param name="Section">Section name to be read from</param>
        /// <param name="Key">Key name value to be returned</param>
        /// <returns>Value of the given key in Ini file</returns>
        private string IniReadValue(string Section, string Key)
        {
            StringBuilder returnValue = new StringBuilder(MAX_SIZE);
            GetPrivateProfileString(Section, Key, "", returnValue, MAX_SIZE, this.FilePath);
            return returnValue.ToString();
        }

        /// <summary>
        /// This method will read the given section in an Ini file
        /// </summary>
        /// <param name="sectionName">Name of the section to be read</param>
        /// <param name="sectionSize">Size of the section</param>
        /// <returns>Section data</returns>
        private string IniReadSection(string sectionName, out int sectionSize)
        {
            byte[] sectionData = new byte[1];
            int maxSize;
            int iterationSize;

            // Get all values in this section, making sure to allocate enough space            
            //The return value of GetPrivateProfileSection specifies the number of characters copied to the buffer, 
            //not including the terminating null character. 
            //If the buffer is not large enough to contain all the key name 
            //and value pairs associated with the named section, the return value is equal to nSize minus two
            maxSize = INITIAL_BUFFER_SIZE / 2;
            do
            {
                maxSize = maxSize * 2;
                iterationSize = maxSize;
                sectionData = new byte[maxSize];
                iterationSize = GetPrivateProfileSection(sectionName, sectionData, maxSize, FilePath);
            } while (iterationSize != 0 && iterationSize == maxSize - 2);
            sectionSize = iterationSize;
            return Encoding.ASCII.GetString(sectionData, 0, iterationSize); //sectionData;//
        }

        #region Translatable attributes

        /// <summary>
        /// This function loads Ini section for the first time. And returns attribute value.
        /// </summary>
        /// <param name="attributeName">Name of attribute</param>
        /// <returns>Either Translatable / Non-Translatabale.</returns>
        public AttributeType GetAttributeType(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName");

            if (_IniAttributeValue == null)
            {
                LoadIniAttributeSection();
            }

            if (!IsCaseSensitive)
            {
                attributeName = attributeName.ToLower();
            }

            if (_IniAttributeValue.Contains(attributeName))
            {
                return AttributeType.Translatable;
            }
            else
            {
                return AttributeType.NonTranslatable;
            }

        }

        /// <summary>
        /// This method will Load the Ini "Translatable Attributes" section in memory with tags name
        /// </summary>
        private void LoadIniAttributeSection()
        {
            int tagSectionSize;
            _IniAttributeValue = new List<string>();

            string attSections = IniReadSection("Attributes", out tagSectionSize);
            if (attSections.Length == 0)
            {
                //NO need to translate attributes.
                return;
            }
            string attributeName = string.Empty;
            char[] charNull = { '\0' };
            string[] attLines = attSections.Split(charNull);
            for (int i = 0; i < attLines.Length; i++)
            {
                attributeName = attLines[i].ToString();
                if (!IsCaseSensitive)
                {
                    attributeName = attributeName.ToLower();
                }
                if (attributeName != string.Empty)
                    _IniAttributeValue.Add(attributeName);
            }
        }
        #endregion

        #region Two New Ini types
        /// <summary>
        /// This method will Load the Ini "Tags" section in memory with tags name and its type (external, interanal)
        /// Rules below:
        /// For Trados Ini: 
        /// Adds TagType.Internal value for Internal
        /// Adds TagType.External value for External.
        /// Adds TagType.InternalGroup value for Internal,Group 
        /// Adds TagType.ExternalGroup value for External,Group.
        /// For NON-Translatable tags and translatable attributes Ini: 
        /// Adds TagType.InternalGroup value for Internal 
        /// Adds TagType.ExternalGroup value for External.
        /// For Translatable tags and translatable attributes Ini: 
        /// Adds TagType.Internal value for Internal 
        /// Adds TagType.External value for External.
        /// </summary>
        private void LoadIniTagSection()
        {
            int tagSectionSize;
            _IniTagsKeyValue = new Dictionary<string, TagType>();

            string tagsSections = IniReadSection("Tags", out tagSectionSize);

            //This condition is valid for trados file type only. 
            // if (tagsSections.Length == 0 & _IniFileType == IniFileType.Trados)
            if (tagsSections.Length == 0)
            {
                throw new ApplicationException("Tags section is not found in Ini file");
            }

            char[] charNull = { '\0' };
            char[] lineSplit = { '=', ':', ',', ' ' };
            string[] tagLines = tagsSections.Split(charNull);
            string tagName = string.Empty;
            string basicTagType = string.Empty;
            for (int i = 0; i < tagLines.Length; i++)
            {
                //Remove the attribute based filtering rules defined in [] e.g. - In "Tag19=span[@class="keyword"]:Internal,Group" remove "[@class="keyword"]"
                tagLines[i] = Regex.Replace(tagLines[i], @"\[.*\]", "");

                string[] splittedItems = tagLines[i].Split(lineSplit);
                //Assumptions: 
                //1) spilttedItems[0] will always be key (say Tag1,Tag2...)and rest will be its value (say simpleLink:Internal,Group) 
                //2) TagType defined in Ini file will be case insensitive i.e Ini extrenal=External=EXTERNAL
                //3) TagName to be serach will be case sensitive i.e. Ini localizedText != localizedtext != LocalizedText
                if (splittedItems.Length > 2)
                {
                    tagName = splittedItems[1].ToString().Trim().Replace("&colon;", ":");
                    if (!IsCaseSensitive)
                    {
                        tagName = tagName.ToLower();
                    }

                    // If the tag name not already added to the mapper
                    if (!_IniTagsKeyValue.ContainsKey(tagName))
                    {
                        basicTagType = splittedItems[2].Trim(); 
                        if (basicTagType.Equals("external", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.NonTranslatableElements)
                        {
                            _IniTagsKeyValue.Add(tagName, TagType.ExternalGroup);
                        }
                        else if (basicTagType.Equals("internal", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.NonTranslatableElements)
                        {
                            _IniTagsKeyValue.Add(tagName, TagType.InternalGroup);
                        }
                        else if (basicTagType.Equals("external", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.TranslatableElements)
                        {
                            _IniTagsKeyValue.Add(tagName, TagType.External);
                        }
                        else if (basicTagType.Equals("internal", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.TranslatableElements)
                        {
                            _IniTagsKeyValue.Add(tagName, TagType.Internal);
                        }
                        else if (basicTagType.Equals("external", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.Trados)
                        {
                            //if ((splittedItems.Length > 3) && (splittedItems[3].Trim().ToLower().Equals("group")))
                            if (CheckTagType(splittedItems, "group") == true)
                            {
                                _IniTagsKeyValue.Add(tagName, TagType.ExternalGroup);
                            }
                            else
                            {
                                _IniTagsKeyValue.Add(tagName, TagType.External);
                            }
                        }
                        else if (basicTagType.Equals("internal", StringComparison.InvariantCultureIgnoreCase) && FileType == IniFileType.Trados)
                        {
                            // if ((splittedItems.Length > 3) && (splittedItems[3].Trim().ToLower().Equals("group")))
                            if (CheckTagType(splittedItems, "group") == true)
                            {
                                _IniTagsKeyValue.Add(tagName, TagType.InternalGroup);
                            }
                            else
                            {
                                _IniTagsKeyValue.Add(tagName, TagType.Internal);
                            }
                        }
                        /*
                    else
                    {
                        _IniTagsKeyValue.Add(tagName, TagType.NotDefined);
                    }*/
                    }// End If
                }
            }
        }
        #endregion

        /// <summary>
        /// This function checks defaultTagStyle in Ini file and returns the tag type depending on rules defined below:
        /// If type=Trados/ NON-Trans, possible valid values are: external/Internal. 
        /// If any one of the values is not defined[This also inclused if string is empty], Internal is sent
        /// then Internal is sent by default.
        /// If type=Trans tag, possible valid values are: externalgroup/Internalgroup. 
        /// If any one of the values is not defined[This also inclused if string is empty], ExternalGroup is sent.
        /// </summary>
        /// <returns>Default TagType (Internal/External/ExternalGroup depending on the above rules.)</returns>
        private TagType GetDefaultTagStyle()
        {
            string defaultTagStyle = string.Empty;
            defaultTagStyle = IniReadValue("Tags", "DefaultTagStyle");

            //File type is trados/non translatable elements
            if (FileType == IniFileType.Trados || FileType == IniFileType.NonTranslatableElements)
            {
                if (defaultTagStyle.Trim().Equals("external", StringComparison.InvariantCultureIgnoreCase)) //FxCop       // If external
                {
                    return TagType.External;
                }
                else if (defaultTagStyle.Trim().Equals("internal", StringComparison.InvariantCultureIgnoreCase)) //FxCop    // If Internal
                {
                    return TagType.Internal;
                }
                return TagType.Internal;            // If empty or not Internal/External.
            }
            else if (FileType == IniFileType.TranslatableElements)
            {
                if (defaultTagStyle.Trim().Equals("external,group", StringComparison.InvariantCultureIgnoreCase)) //FxCop 
                    return TagType.ExternalGroup;
                else if (defaultTagStyle.Trim().Equals("internal,group", StringComparison.InvariantCultureIgnoreCase)) //FxCop ))
                    return TagType.InternalGroup;
                else
                    return TagType.ExternalGroup;   // If empty or not Internal/External.
            }
            return TagType.Internal;
        }

        /// <summary>
        /// This function extract the Ini file type. The type name should be: IniType. If tagtype is not defined,
        /// system returns NotDefined tah type.
        /// </summary>
        /// <returns>Tag type.</returns>
        private IniFileType GetIniFileType()
        {
            string strIniFileType = string.Empty;

            int tagSectionSize;
            string genrealSections = IniReadSection("GeneralSGMLSettings", out tagSectionSize);

            //This condition is valid for trados file type only. 
            // if (tagsSections.Length == 0 & _IniFileType == IniFileType.Trados)
            if (genrealSections.Length == 0)
            {
                throw new ApplicationException("GeneralSGMLSettings section is not found in Ini file");
            }

            strIniFileType = IniReadValue("GeneralSGMLSettings", "IniType");

            //TO DO: Set default depending on the file.
            switch (strIniFileType.ToLower())
            {
                case "nontranslatable elements and translatable attributes":
                    FileType = IniFileType.NonTranslatableElements;
                    break;
                case "translatable elements and translatable attributes":
                    FileType = IniFileType.TranslatableElements;
                    break;
                default:
                    FileType = IniFileType.Trados;
                    break;
            }

            return FileType;

        }
    }
}
