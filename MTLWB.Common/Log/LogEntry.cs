using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
namespace MTLWB.Common.Log
{
    internal class LogEntry
    {

        /// <summary>Log Level</summary>
        public LogType Level { get; set; }

        /// <summary>The Message to log</summary>
        public string Message { get; set; }

        /// <summary>Exception to log entry</summary>
        public Exception LoggedException { get; set; }

        /// <summary>Gets the time of the entry creation</summary>
        public DateTime TimeStamp { get { return _timeStamp; } }
        private DateTime _timeStamp = DateTime.Now;

        public LogEntry(LogType level, string message)
        {
            Level = level;
            Message = message;
        }

        public LogEntry(LogType level, Exception exception)
        {
            Level = level;
            LoggedException = exception;
        }

        public LogEntry(LogType level, string message, Exception exception)
        {
            Level = level;
            Message = message;
            LoggedException = exception;
        }

        public string ToConsoleString()
        {
            StringBuilder sb = new StringBuilder();

            if (Level == LogType.Error)
                sb.Append("Error: ");
            if (!string.IsNullOrEmpty(Message))
            {
                sb.Append(Message);
                sb.Append(' ');
            }

            if (LoggedException != null)
            {
                sb.Append('[');
                sb.Append(LoggedException.Message);
                sb.Append(']');
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Level.ToString());
            sb.Append(':');
            
            sb.Append(TimeStamp.ToString("MM/dd/yyyy HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            sb.Append('|');
            
            if(!string.IsNullOrEmpty(Message))
                sb.Append(Message);

            if (LoggedException != null)
                WriteFormattedException(sb, LoggedException);

            return sb.ToString();
        }

        /// <summary>
        /// Creates a readable message from an exception and it's child exceptions
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        internal static void WriteFormattedException(StringBuilder sb, Exception exception)
        {
            // Add to the stack of each nested exception in the reverse order
            // That way the most inner exception is at the top
            int maxExceptionDepth = 10; //just in case of loop.
            Stack<Exception> nestedExceptions = new Stack<Exception>();
            nestedExceptions.Push(exception);
            exception = exception.InnerException;
            while (exception != null && --maxExceptionDepth > 0)
            {
                nestedExceptions.Push(exception);
                exception = exception.InnerException;
                maxExceptionDepth--;
            }

            while (nestedExceptions.Count > 0)
            {
                exception = nestedExceptions.Pop();
                string exceptionText = string.Format("{0}(\"{1}\") Source: '{2}' Stack='{3}'", exception.GetType().Name, exception.Message, exception.Source, exception.StackTrace);
                sb.Append("Exception=\"");
                sb.Append(exceptionText);
                sb.Append("\" ");
            }
        }
    }
}
