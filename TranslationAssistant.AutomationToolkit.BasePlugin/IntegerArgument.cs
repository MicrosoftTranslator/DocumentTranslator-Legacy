// // ----------------------------------------------------------------------
// // <copyright file="IntegerArgument.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>IntegerArgument.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System.Globalization;

    /// <summary>
    ///     The integer argument.
    /// </summary>
    public class IntegerArgument : Argument
    {
        #region Fields

        /// <summary>
        ///     The value separators.
        /// </summary>
        private readonly char[] valueSeparators;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerArgument" /> class.
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
        public IntegerArgument(string name, bool required, string description)
            : this(name, required, null, description)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerArgument" /> class.
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
        public IntegerArgument(string name, bool required, char[] separators, string description)
            : this(name, required, new int[] { }, new int[] { }, separators, description)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerArgument" /> class.
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
        public IntegerArgument(
            string name,
            bool required,
            int[] defaultValues,
            int[] validValues,
            char[] separators,
            string description)
            : base(name, required, new string[] { }, new string[] { }, true, description)
        {
            if (defaultValues != null && defaultValues.Length > 0)
            {
                foreach (int i in defaultValues)
                {
                    this.ArgumentDefaults.Add(i.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (validValues != null && validValues.Length > 0)
            {
                foreach (int i in validValues)
                {
                    this.ArgumentValidValues.Add(i.ToString(CultureInfo.InvariantCulture));
                }
            }

            this.valueSeparators = null;
            if (separators != null)
            {
                this.valueSeparators = separators;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get value integers.
        /// </summary>
        /// <returns>
        ///     The <see cref="int[]" />.
        /// </returns>
        public int[] GetValueIntegers()
        {
            var list = new int[this.Values.Count];
            for (int i = 0; i < this.Values.Count; i++)
            {
                list[i] = int.Parse((string)this.Values[i], CultureInfo.InvariantCulture);
            }

            return list;
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        /// <param name="Logger">
        ///     The logger.
        /// </param>
        public override void Usage(ILogger Logger)
        {
            string line = string.Empty;
            if (this.valueSeparators != null && this.valueSeparators.Length > 0)
            {
                line = string.Format(
                    CultureInfo.InvariantCulture,
                    ", or integers separated by the following characters: ['{0}'",
                    this.valueSeparators[0]);
                for (int i = 1; i < this.valueSeparators.Length; i++)
                {
                    line = string.Format(CultureInfo.InvariantCulture, "{0},'{1}'", line, this.valueSeparators[i]);
                }

                line = string.Format(CultureInfo.InvariantCulture, "{0}]", line);
            }

            this.Usage(
                Logger,
                string.Format(CultureInfo.InvariantCulture, "User can only supply integer to this argument{0}.", line));
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
                        // Double check the values (in the if condition) to make sure
                        // it is integer, in case we don't have
                        // any valid values to check against.
                        // int tmp = Int32.Parse( argValue, CultureInfo.InvariantCulture );
                        int.Parse(argValue, CultureInfo.InvariantCulture);
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