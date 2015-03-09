// // ----------------------------------------------------------------------
// // <copyright file="LoggingManager.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>LoggingManager.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business
{
    #region

    using System;
    using System.IO;

    using global::TranslationAssistant.AutomationToolkit.BasePlugin;

    #endregion

    public static class LoggingManager
    {
        #region Public Methods and Operators

        public static void LogError(string message)
        {
            var timestamp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " : ";
            var messageToLog = timestamp + message;
            File.AppendAllText("Log.txt", messageToLog + Environment.NewLine);
            new ConsoleLogger().WriteLine(LogLevel.Error, messageToLog);
        }

        public static void LogMessage(string message)
        {
            var timestamp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " : ";
            var messageToLog = timestamp + message;
            File.AppendAllText("Log.txt", timestamp + messageToLog + Environment.NewLine);
            new ConsoleLogger().WriteLine(LogLevel.Msg, messageToLog);
        }

        #endregion
    }
}