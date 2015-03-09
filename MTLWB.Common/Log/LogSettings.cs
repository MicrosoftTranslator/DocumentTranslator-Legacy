using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTLWB.Common.Log
{
    /// <summary>
    /// Holds the settings for a collections of loggers.
    /// </summary>
    public class LogSettings
    {
        /// <summary>
        /// Gets or Sets the rules for the console logger.
        /// </summary>
        public LogType[] ConsoleLogRule { get; set; }

        private List<LogBase> fileLoggers = new List<LogBase>();
        
        /// <summary>
        /// Gets the collection of loggers added.
        /// </summary>
        public LogBase[] Loggers
        {
            get
            {
                List<LogBase> loggers = new List<LogBase>();
                if (ConsoleLogRule != null)
                    loggers.Add(new ConsoleLogger(ConsoleLogRule));
                if (fileLoggers.Count > 0)
                    loggers.AddRange(fileLoggers);

                return loggers.ToArray();
            }
        }

        /// <summary>
        /// Adds a file logger to the collection of loggers.
        /// </summary>
        /// <param name="fileName">Name of the log file.</param>
        /// <param name="logFilePath">path of the directory where log file is to be created.</param>
        /// <param name="logTypes">Array of log types to be written to this log file.</param>
        /// <param name="logFileSizeMb">Max size in MB for this log file.</param>
        public void AddFileLog(string fileName, string logFilePath, LogType[] logTypes, int logFileSizeMb)
        {
            fileLoggers.Add(new FileLogger(logTypes, fileName, logFilePath, logFileSizeMb));
        }
    }
}
