// // ----------------------------------------------------------------------
// // <copyright file="SimpleStringArgument.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>SimpleStringArgument.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    /// <summary>
    ///     The simple string argument.
    /// </summary>
    public class SimpleStringArgument : Argument
    {
        #region Fields

        /// <summary>
        ///     The value separators.
        /// </summary>
        private readonly char[] valueSeparators;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleStringArgument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="separators">
        ///     The separators.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public SimpleStringArgument(string name, bool required, char[] separators, string description)
            : this(name, required, new string[] { }, separators, description)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleStringArgument" /> class.
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
        /// <param name="separators">
        ///     The separators.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public SimpleStringArgument(
            string name,
            bool required,
            string[] defaultValues,
            char[] separators,
            string description)
            : base(name, required, defaultValues, new string[] { }, true, description)
        {
            this.valueSeparators = null;
            if (separators != null)
            {
                this.valueSeparators = separators;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleStringArgument" /> class.
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
        /// <param name="separators">
        ///     The separators.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public SimpleStringArgument(
            string name,
            bool required,
            string[] defaultValues,
            string[] validValues,
            char[] separators,
            string description)
            : base(name, required, defaultValues, validValues, true, description)
        {
            this.valueSeparators = null;
            if (separators != null)
            {
                this.valueSeparators = separators;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The parse.
        /// </summary>
        /// <param name="argumentText">
        ///     The argument text.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected override bool Parse(string argumentText)
        {
            // We don't use this.Values here because
            // we may end up keep adding values to
            // default list instead.
            if (this.BasicValidation(argumentText))
            {
                string argValueString =
                    argumentText.Substring(
                        argumentText.IndexOf(ArgumentGetNameValueSeparator) + ArgumentGetNameValueSeparator.Length);
                string[] argValues = { argValueString };

                // split the values if we have a separator.
                if (this.valueSeparators != null && this.valueSeparators.Length > 0)
                {
                    argValues = argValueString.Split(this.valueSeparators);
                }

                foreach (string argValue in argValues)
                {
                    if (this.ValidationAgainstValidValues(argValue))
                    {
                        this.ArgumentValues.Add(argValue);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}