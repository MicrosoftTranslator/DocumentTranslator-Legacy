// // ----------------------------------------------------------------------
// // <copyright file="Argument.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>Argument.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The argument.
    /// </summary>
    public class Argument
    {
        #region Constants

        /// <summary>
        ///     The argument get name value separator.
        /// </summary>
        public const string ArgumentGetNameValueSeparator = ":";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public Argument(string name, bool required, string description)
            : this(name, required, null, new string[] { }, true, description)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="defaultValues">
        ///     The default values.
        /// </param>
        /// <param name="enumType">
        ///     The enumeration type.
        /// </param>
        /// <param name="ignoreCase">
        ///     The ignore case.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public Argument(
            string name,
            bool required,
            string[] defaultValues,
            Type enumType,
            bool ignoreCase,
            string description)
            : this(name, required, defaultValues, new string[] { }, ignoreCase, description)
        {
            foreach (string e in Enum.GetNames(enumType))
            {
                this.ArgumentValidValues.Add(e);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="defaultValues">
        ///     The default values.
        /// </param>
        /// <param name="validValues">
        ///     The valid values.
        /// </param>
        /// <param name="ignoreCase">
        ///     The ignore case.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public Argument(
            string name,
            bool required,
            string[] defaultValues,
            string[] validValues,
            bool ignoreCase,
            string description)
        {
            this.Name = name;
            this.Required = required;
            this.IgnoreCase = ignoreCase;
            this.Description = description;
            this.ArgumentValues = new ArrayList();
            if (defaultValues == null || defaultValues.Length <= 0)
            {
                this.ArgumentDefaults = new ArrayList();
            }
            else
            {
                this.ArgumentDefaults = new ArrayList(defaultValues);
            }

            if (validValues == null || validValues.Length <= 0)
            {
                this.ArgumentValidValues = new ArrayList();
            }
            else
            {
                this.ArgumentValidValues = new ArrayList(validValues);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Gets the value string.
        /// </summary>
        public virtual string ValueString
        {
            get
            {
                return string.Join(",", (string[])this.Values.ToArray(typeof(string)));
            }
        }

        /// <summary>
        ///     Gets the values.
        /// </summary>
        public virtual ArrayList Values
        {
            get
            {
                if (this.ArgumentDefaults.Count > 0 && this.ArgumentValues.Count <= 0)
                {
                    return this.ArgumentDefaults;
                }

                return this.ArgumentValues;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the argument defaults.
        /// </summary>
        protected ArrayList ArgumentDefaults { get; set; }

        /// <summary>
        ///     Gets or sets the argument valid values.
        /// </summary>
        protected ArrayList ArgumentValidValues { get; set; }

        /// <summary>
        ///     Gets or sets the argument values.
        /// </summary>
        protected ArrayList ArgumentValues { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether ignore case.
        /// </summary>
        protected bool IgnoreCase { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The split argument values.
        /// </summary>
        /// <param name="separators">
        ///     The separators.
        /// </param>
        /// <param name="argument">
        ///     The argument.
        /// </param>
        public static void SplitArgumentValues(char[] separators, Argument argument)
        {
            int last = argument.Values.Count;
            for (int i = 0; i < last; i++)
            {
                var argValue = (string)argument.Values[i];
                if (argValue.IndexOfAny(separators) >= 0)
                {
                    string[] argValues = argValue.Split(separators);
                    argument.Values[i] = argValues[0];
                    var newValues = new string[argValues.Length - 1];
                    Array.Copy(argValues, 1, newValues, 0, newValues.Length);
                    argument.Values.AddRange(newValues);
                }
            }
        }

        /// <summary>
        ///     The verify boolean.
        /// </summary>
        /// <param name="argument">
        ///     The argument.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool VerifyBoolean(Argument argument)
        {
            bool valuesValid = true;

            // Declare a regular expression comprising of strings "true" or "false"
            var booleanRegex = new Regex("true|false");

            foreach (string argValue in argument.Values)
            {
                // Matching the user specified string against the regular expression
                Match m = booleanRegex.Match(argValue);
                if (!m.Success)
                {
                    valuesValid = false;
                }
            }

            return valuesValid;
        }

        /// <summary>
        ///     The verify function.
        /// </summary>
        /// <param name="argument">
        ///     The argument.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool VerifyInt32(Argument argument)
        {
            // TO BE DONE : USE regex instead of Parse call
            foreach (string argValue in argument.Values)
            {
                int.Parse(argValue, CultureInfo.InvariantCulture);
            }

            return true;
        }

        /// <summary>
        ///     The parse and consume.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ParseAndConsume(string[] arguments)
        {
            bool result = false;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (this.Parse(arguments[i]))
                {
                    arguments[i] = null;
                    result = true;
                }
            }

            return result || !this.Required;
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        public virtual void Usage(ILogger logger)
        {
            this.Usage(logger, string.Empty);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The basic validation.
        /// </summary>
        /// <param name="argumentText">
        ///     The argument text.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected bool BasicValidation(string argumentText)
        {
            if (argumentText != null)
            {
                if ((argumentText.StartsWith("-") || argumentText.StartsWith("\\") || argumentText.StartsWith("/"))
                    && argumentText.IndexOf(ArgumentGetNameValueSeparator) >= 0)
                {
                    string argName =
                        argumentText.Substring(1, argumentText.IndexOf(ArgumentGetNameValueSeparator) - 1)
                            .ToLower(CultureInfo.InvariantCulture)
                            .Trim();
                    if (this.Name.ToLower(CultureInfo.InvariantCulture).Trim().Equals(argName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     The parse.
        /// </summary>
        /// <param name="argumentText">
        ///     The argument text.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected virtual bool Parse(string argumentText)
        {
            // We don't use this.Values here because
            // we may end up keep adding values to
            // default list instead.
            if (this.BasicValidation(argumentText))
            {
                string argValue =
                    argumentText.Substring(
                        argumentText.IndexOf(ArgumentGetNameValueSeparator) + ArgumentGetNameValueSeparator.Length);
                if (this.ValidationAgainstValidValues(argValue))
                {
                    this.ArgumentValues.Add(argValue);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <param name="extraInfo">
        ///     The extra info.
        /// </param>
        protected void Usage(ILogger logger, string extraInfo)
        {
            logger.WriteLine(LogLevel.Msg, string.Empty);
            logger.Write(LogLevel.Msg, "  {0,-20}:", this.Name);
            this.PrintParagraph(logger, this.Description, true);

            // Print extra info ( if we have any )
            if (!string.IsNullOrEmpty(extraInfo))
            {
                logger.WriteLine(LogLevel.Msg, string.Empty);
                this.PrintParagraph(logger, extraInfo, false);
            }

            // Print info if this argument is Required.
            if (this.Required)
            {
                logger.WriteLine(LogLevel.Msg, string.Empty);
                this.PrintParagraph(logger, "Required.", false);
            }

            // Print valid values.
            if (this.ArgumentValidValues.Count > 0)
            {
                logger.WriteLine(LogLevel.Msg, string.Empty);
                logger.Write(LogLevel.Msg, "{0,23}", " ");
                logger.WriteLine(
                    LogLevel.Msg,
                    "Valid Values:[{0}]",
                    string.Join(",", (string[])this.ArgumentValidValues.ToArray(typeof(string))));
            }

            // Print default values.
            if (this.ArgumentDefaults.Count > 0)
            {
                logger.WriteLine(LogLevel.Msg, string.Empty);
                logger.Write(LogLevel.Msg, "{0,23}", " ");
                logger.WriteLine(
                    LogLevel.Msg,
                    "Default:[{0}]",
                    string.Join(",", (string[])this.ArgumentDefaults.ToArray(typeof(string))));
            }
        }

        /// <summary>
        ///     The validation against valid values.
        /// </summary>
        /// <param name="value1">
        ///     The value 1.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected bool ValidationAgainstValidValues(string value1)
        {
            // Need to see if the value is valid.
            if (this.ArgumentValidValues.Count <= 0)
            {
                return true;
            }

            string targetValue = value1;
            foreach (string validValue in this.ArgumentValidValues)
            {
                string tmp = validValue;
                if (this.IgnoreCase)
                {
                    tmp = tmp.ToLower(CultureInfo.InvariantCulture).Trim();
                    targetValue = targetValue.ToLower(CultureInfo.InvariantCulture).Trim();
                }

                if (tmp == targetValue)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     The print paragraph.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="doNotIndentFirstLine">
        ///     The do not indent first line.
        /// </param>
        private void PrintParagraph(ILogger logger, string text, bool doNotIndentFirstLine)
        {
            string line = string.Empty;
            bool firstTime = doNotIndentFirstLine;
            string[] words = text.Split(new[] { ' ' });
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    if (line.Length + 1 + word.Length > 35)
                    {
                        // Print current line, and start
                        // a new line.
                        if (!firstTime)
                        {
                            logger.Write(LogLevel.Msg, "{0,23}", " ");
                        }

                        logger.WriteLine(LogLevel.Msg, line);
                        line = word + " ";
                        firstTime = false;
                    }
                    else
                    {
                        // We add the current word into the line.
                        line += word + " ";
                    }
                }
            }

            // Print last line.
            if (!firstTime)
            {
                logger.Write(LogLevel.Msg, "{0,23}", " ");
            }

            logger.WriteLine(LogLevel.Msg, line);
        }

        #endregion
    }
}