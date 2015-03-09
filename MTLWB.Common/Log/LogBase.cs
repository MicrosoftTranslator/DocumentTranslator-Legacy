using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTLWB.Common.Log
{
    /// <summary>
    /// The abstract base class for all the logger classes.
    /// </summary>
    public abstract class LogBase : IDisposable
    {
        /// <summary>Used for locking</summary>
        protected static object _lock = new object();

        /// <summary>Levels to log. Null means all</summary>
        private LogType[] Levels { get; set; }

        /// <summary>
        /// Initializes a new instance of LogBase class with specified log types.
        /// </summary>
        /// <param name="levels">An array of types of log.</param>
        public LogBase(LogType[] levels)
        {
            Levels = levels;
        }

        /// <summary>
        /// Logs an entry if it needs to be logged</summary>
        /// <param name="entry">Log entry to be logged.</param>
        internal void Log(LogEntry entry)
        {
            if (Matches(entry))
            {
                LogInternal(entry);
            }
        }

        /// <summary>
        /// Performs actual logging</summary>
        /// <param name="entry"></param>
        internal abstract void LogInternal(LogEntry entry);

        /// <summary>
        /// Returns true if a particular log entry needs to be loggeed
        /// </summary>
        /// <returns></returns>
        private bool Matches(LogEntry entry)
        {
            if (Levels != null && !Levels.Contains(entry.Level))
                return false;

            return true;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>to be overriden</summary>
        protected abstract void Dispose(bool disposing);

    }
}
