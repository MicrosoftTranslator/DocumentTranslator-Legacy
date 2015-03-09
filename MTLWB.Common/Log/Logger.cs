using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTLWB.Common.Log
{
    /// <summary>
    /// Provides static methods to log exceptions and status messages.
    /// </summary>
    public static class Logger
    {
        /// <summary>used to lock thread operations</summary>
        private static object _lock = new object();

        /// <summary> If true then the module has been initialized</summary>
        private static bool Initialized { get; set; }

        /// <summary>Private logger instances</summary>
        private static LogBase[] Loggers { get; set; }

        /// <summary>
        /// Initializes loggers with the default settings (only console logger)
        /// </summary>
        public static void Initialize()
        {
            lock (_lock)
            {
                Loggers = new LogBase[] { new ConsoleLogger(new LogType[] { LogType.Status, LogType.Warning, LogType.Error }) };
            }
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                foreach (var logger in Loggers)
                {
                    logger.Dispose();
                }
            }
        }

        /// <summary>
        /// Initializes loggers with the specified log settings.
        /// </summary>
        /// <param name="logSettings">Log settings indicating the loggers to be initialized.</param>
        public static void Initialize(LogSettings logSettings)
        {
            lock (_lock)
            {
                Loggers = logSettings.Loggers;
            }
        }

        internal static void LogInternal(LogEntry logEntry)
        {
            lock (_lock)
            {
                foreach (var logger in Loggers)
                {
                    logger.Log(logEntry);
                }
            }
        }

        /// <summary>
        /// Logs the specified message with the specified log type.
        /// </summary>
        /// <param name="logType">Indicates the type of log.</param>
        /// <param name="message">Message to be logged.</param>
        public static void Log(LogType logType, string message)
        {
            LogInternal(new LogEntry(logType, message));
        }

        /// <summary>
        /// Logs the specified exception with the specified log type.
        /// </summary>
        /// <param name="logType">Indicates the type of log.</param>
        /// <param name="exception">Exception to be logged.</param>
        public static void Log(LogType logType, Exception exception)
        {
            LogInternal(new LogEntry(logType, exception));
        }

        /// <summary>
        /// Logs the specified message and exception with the specified log type.
        /// </summary>
        /// <param name="logType">Indicates the type of log.</param>
        /// <param name="message">Message to be logged.</param>
        /// <param name="exception">Exception to be logged.</param>
        public static void Log(LogType logType, string message, Exception exception)
        {
            LogInternal(new LogEntry(logType, message, exception));
        }

        private static void CheckInitialized()
        {
            lock (_lock)
            {
                if (!Initialized)
                    Initialize();
            }
        }

    }
}
