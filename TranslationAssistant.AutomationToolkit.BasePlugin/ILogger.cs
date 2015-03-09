// // ----------------------------------------------------------------------
// // <copyright file="ILogger.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>ILogger.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.AutomationToolkit.BasePlugin
{
    using System;

    /// <summary>
    ///     The Logger interface.
    /// </summary>
    public interface ILogger
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The write.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="msg">
        ///     The msg.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        void Write(LogLevel level, string msg, params object[] arguments);

        /// <summary>
        ///     The write exception.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        void WriteException(Exception ex);

        /// <summary>
        ///     The write line.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="msg">
        ///     The msg.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        void WriteLine(LogLevel level, string msg, params object[] arguments);

        #endregion
    }
}