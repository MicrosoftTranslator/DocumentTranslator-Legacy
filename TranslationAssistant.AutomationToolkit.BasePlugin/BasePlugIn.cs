// // ----------------------------------------------------------------------
// // <copyright file="BasePlugIn.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>BasePlugIn.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    /// <summary>
    ///     The base plug in.
    /// </summary>
    public abstract class BasePlugIn
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePlugIn" /> class.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        protected BasePlugIn(ILogger logger)
        {
            this.Arguments = new ArgumentList(null, logger);
            this.Logger = logger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the arguments.
        /// </summary>
        protected ArgumentList Arguments { get; set; }

        /// <summary>
        ///     Gets or sets the logger.
        /// </summary>
        protected ILogger Logger { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public abstract bool Execute();

        /// <summary>
        ///     The parse.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool Parse(string[] args)
        {
            return this.Parse(args, new Argument[] { });
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        public virtual void Usage()
        {
            this.Logger.WriteLine(LogLevel.Msg, "Plugin: {0}", this.Name);
            this.Logger.WriteLine(LogLevel.Msg, string.Empty);
            this.Logger.WriteLine(LogLevel.Msg, "{0}", this.Description);
            this.Logger.WriteLine(LogLevel.Msg, string.Empty);
            this.Logger.WriteLine(LogLevel.Msg, "==== Argument Descriptions ===================");
            this.Logger.WriteLine(LogLevel.Msg, "All Arguments are of the form /<argName>:<argValue>");
            this.Logger.WriteLine(LogLevel.Msg, string.Empty);
            this.Arguments.Usage(this.Logger);
            this.Logger.WriteLine(LogLevel.Msg, string.Empty);
            this.Logger.WriteLine(LogLevel.Msg, string.Empty);
        }

        #endregion

        #region Methods

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
        protected bool Parse(string[] args, Argument[] extraArguments)
        {
            bool result = this.Arguments.Parse(args, extraArguments);
            return result;
        }

        #endregion
    }
}