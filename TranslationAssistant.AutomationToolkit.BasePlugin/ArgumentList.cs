// // ----------------------------------------------------------------------
// // <copyright file="ArgumentList.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>ArgumentList.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System.Collections;
    using System.Globalization;

    /// <summary>
    ///     The argument list.
    /// </summary>
    public class ArgumentList
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        ///     The internal ordered arg names.
        /// </summary>
        private readonly ArrayList internalOrderedArgNames;

        /// <summary>
        ///     The list.
        /// </summary>
        private readonly Hashtable list;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArgumentList" /> class.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        public ArgumentList(Argument[] args, ILogger logger)
        {
            this.list = new Hashtable();
            this.internalOrderedArgNames = new ArrayList();
            if (args != null)
            {
                this.Set(args);
            }

            this.Logger = logger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="Argument" />.
        /// </returns>
        public Argument Get(string name)
        {
            if (!this.list.ContainsKey(name.ToLower(CultureInfo.InvariantCulture).Trim()))
            {
                return null;
            }

            return (Argument)this.list[name.ToLower(CultureInfo.InvariantCulture).Trim()];
        }

        /// <summary>
        ///     The get arguments.
        /// </summary>
        /// <returns>
        ///     The <see cref="ICollection" />.
        /// </returns>
        public ICollection GetArguments()
        {
            return this.list.Values;
        }

        /// <summary>
        ///     The parse.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="extraArguments">
        ///     The extra arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Parse(string[] args, Argument[] extraArguments)
        {
            var internalArgs = (string[])args.Clone();
            foreach (Argument argument in this.list.Values)
            {
                if (!argument.ParseAndConsume(internalArgs))
                {
                    this.Logger.WriteLine(
                        LogLevel.Error,
                        "ERROR: Argument [{0}] did not get a valid value from user.",
                        argument.Name);
                    return false;
                }

                this.Logger.WriteLine(
                    LogLevel.Debug,
                    "Argument [{0}] has value [{1}].",
                    argument.Name,
                    argument.ValueString);
            }

            foreach (Argument argument in extraArguments)
            {
                if (!argument.ParseAndConsume(internalArgs))
                {
                    this.Logger.WriteLine(
                        LogLevel.Error,
                        "ERROR: Argument [{0}] did not get a valid value from user.",
                        argument.Name);
                    return false;
                }

                this.Logger.WriteLine(
                    LogLevel.Debug,
                    "Argument [{0}] has value [{1}].",
                    argument.Name,
                    argument.ValueString);
            }

            // If there are any Arguments left we consider
            // it an error too.
            foreach (string argument in internalArgs)
            {
                if (argument != null)
                {
                    this.Logger.WriteLine(LogLevel.Error, "ERROR: Unknown input argument [{0}].", argument);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        public void Usage(ILogger logger)
        {
            foreach (string argName in this.internalOrderedArgNames)
            {
                ((Argument)this.list[argName]).Usage(logger);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="argument">
        ///     The argument.
        /// </param>
        private void Set(Argument argument)
        {
            this.list[argument.Name.ToLower(CultureInfo.InvariantCulture).Trim()] = argument;
            this.internalOrderedArgNames.Add(argument.Name.ToLower(CultureInfo.InvariantCulture).Trim());
        }

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Set(Argument[] args)
        {
            foreach (Argument argument in args)
            {
                this.Set(argument);
            }
        }

        #endregion
    }
}