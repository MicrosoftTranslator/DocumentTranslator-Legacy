// // ----------------------------------------------------------------------
// // <copyright file="RegExArgument.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>RegExArgument.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The regex argument.
    /// </summary>
    public class RegexArgument : Argument
    {
        #region Fields

        /// <summary>
        ///     The argument value matches.
        /// </summary>
        private readonly ArrayList argumentValueMatches;

        /// <summary>
        ///     The regex value.
        /// </summary>
        private readonly Regex regexValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexArgument" /> class.
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
        /// <param name="match">
        ///     The match.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public RegexArgument(string name, bool required, string[] defaultValues, Regex match, string description)
            : base(name, required, defaultValues, new string[] { }, false, description)
        {
            this.regexValue = match;
            this.argumentValueMatches = new ArrayList();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get value matches.
        /// </summary>
        /// <returns>
        ///     The <see cref="Match[]" />.
        /// </returns>
        public Match[] GetValueMatches()
        {
            return (Match[])this.argumentValueMatches.ToArray(typeof(Match));
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
                string argValue = argumentText.Substring(argumentText.IndexOf(":") + 1);
                Match match = this.regexValue.Match(argValue);
                if (match.Success && this.ValidationAgainstValidValues(argValue))
                {
                    this.ArgumentValues.Add(argValue);
                    this.argumentValueMatches.Add(match);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}