// // ----------------------------------------------------------------------
// // <copyright file="BooleanArgument.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>BooleanArgument.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System.Globalization;

    /// <summary>
    ///     The boolean argument.
    /// </summary>
    public class BooleanArgument : Argument
    {
        #region Fields

        /// <summary>
        ///     The value separators.
        /// </summary>
        private readonly char[] valueSeparators;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanArgument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public BooleanArgument(string name, bool required, bool defaultValue, string description)
            : this(name, required, defaultValue, null, description)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanArgument" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="required">
        ///     The required.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <param name="separators">
        ///     The separators.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public BooleanArgument(string name, bool required, bool defaultValue, char[] separators, string description)
            : base(
                name,
                required,
                new[] { defaultValue.ToString(CultureInfo.InvariantCulture) },
                new[] { true.ToString(), false.ToString() },
                true,
                description)
        {
            this.valueSeparators = null;
            if (separators != null)
            {
                this.valueSeparators = separators;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get value booleans.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool[]" />.
        /// </returns>
        public bool[] GetValueBooleans()
        {
            var list = new bool[this.Values.Count];
            for (int i = 0; i < this.Values.Count; i++)
            {
                list[i] = bool.Parse((string)this.Values[i]);
            }

            return list;
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