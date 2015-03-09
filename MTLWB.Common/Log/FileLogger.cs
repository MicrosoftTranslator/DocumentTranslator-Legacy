using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;

namespace MTLWB.Common.Log
{
    internal class FileLogger: LogBase
    {
        internal FileLogger(
            LogType[] levels,
            string basename,
            string logFilePath,
            int logFileSizeMb)
            : base(levels)
        {
            Basename = basename;
            LogFilePath = Environment.ExpandEnvironmentVariables(logFilePath);
            MaxLogFileSize = logFileSizeMb;
            FlushTimer = new Timer(FlushWorker, null, 1000, 1000);
            // Register to flush in case process or domain exists
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(DomainUnloadHandler);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExitHandler);

        }

        /// <summary>Default base name of the log file </summary>
        private readonly string Basename = "Log";

        /// <summary>Default Extension for the log file</summary>
        private readonly string LogFileExtension = ".log";

        /// <summary>Path to the log file </summary>
        private readonly string LogFilePath;

        /// <summary>Max log file size in MB</summary>
        private readonly int MaxLogFileSize;

        /// <summary>Current size ouf our written file in byts</summary>
        private int CurrentFileSizeBytes;

        /// <summary>Used to keep track of file counters so that names never collide</summary>
        private int FileCounter;

        /// <summary>Timer for auto-flushing the log to disk based on c_wakeInterval</summary>
        private Timer FlushTimer { get; set; }

        /// <summary>Stream used to write the file</summary>
        private StreamWriter LogStreamWriter { get; set; }

        /// <summary>Stream of the output log file</summary>
        private FileStream LogFileStream { get; set; }

        internal override void LogInternal(LogEntry logEntry)
        {
            lock (_lock)
            {
                try
                {
                    string entry = logEntry.ToString();

                    // check to see if we should roll over to a new log file.
                    if (CurrentFileSizeBytes == 0 || CurrentFileSizeBytes > MaxLogFileSize * 1024 * 1024)
                    {
                        CurrentFileSizeBytes = entry.Length + 2;
                        CreateNewLogFile();
                    }
                    else
                    {
                        CurrentFileSizeBytes += entry.Length + 2;
                    }


                    // Write to stream
                    LogStreamWriter.WriteLine(entry);

                    // flush messages to disk immediately if they indicate trouble
                    if (logEntry.Level >= LogType.Warning)
                    {
                        Flush();
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        WriteLine(LogType.Error, "Failed writing a log entry", e);
                        Flush();
                    }
                    catch
                    {
                        // if we got here we are up the creek and there is nothing we can do...
                    }

                }
            }
        }


        /// <summary>Worker method to flush the log, called by our autoflush timer.</summary>
        private void FlushWorker(object stateInfo)
        {
            try
            {
                Flush();
            }
            catch
            {
                // There's nothing we can really do with this exception
                // but if we don't catch it we'll bring down the app.
            }
        }

        /// <summary>
        /// attempt to flush buffers if we are dying
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExitHandler(object sender, EventArgs e)
        {
            // Write the message to stream
            WriteLine(LogType.Status, "Detected Process Exit event, exiting", null);
            Dispose();
        }

        /// <summary>
        /// attempt to flush buffers if we are dying
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DomainUnloadHandler(object sender, EventArgs e)
        {
            // Write the message to stream
            WriteLine(LogType.Status, "Detected Domain Unload event, exiting", null);
            Dispose();
        }

        /// <summary>Creates a new file.  Opens a writer to it. Cleans up old files if needed</summary>
        private void CreateNewLogFile()
        {
            if (LogStreamWriter != null)
            {
                // Write the message to stream
                WriteLine(LogType.Status, "Creating a new log file...", null);
                Flush();
                LogStreamWriter.Close();
                LogStreamWriter = null;
            }
            if (LogFileStream != null)
            {
                LogFileStream.Close();
                LogFileStream = null;
            }
            string logfilename = GenerateFileName(Basename);
            LogFileStream = new FileStream(logfilename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            LogStreamWriter = new StreamWriter(LogFileStream, Encoding.UTF8);
        }

        private string GenerateFileName(string basename)
        {
            // Cosmos file name format is FileBase_number.
            string name;
            do
            {
                name = string.Format(CultureInfo.InvariantCulture, "{0}_{1}" + LogFileExtension, basename, FileCounter++);
                name = Path.Combine(LogFilePath, name);
            } while (File.Exists(name));
            // Create a file to make sure others can't
            File.WriteAllText(name, "", Encoding.UTF8);
            return name;
        }

        protected void WriteLine(LogType level, string message, Exception exception)
        {
            LogEntry errorLogEntry = new LogEntry(level, message, exception);
            string entry = errorLogEntry.ToString();
            CurrentFileSizeBytes += entry.Length + 2;
            if (LogStreamWriter != null)
            {
                LogStreamWriter.WriteLine(entry);
            }
        }

        /// <summary>
        /// This method flushes the log file to disk.
        /// </summary>
        internal void Flush()
        {
            lock (_lock)
            {
                if (LogStreamWriter != null)
                {
                    LogStreamWriter.Flush();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            // Write the final note to the stream.
            WriteLine(LogType.Status, "Logger is Disposed.", null);
            Flush();
            FlushTimer.Dispose();
            if (LogStreamWriter != null)
                LogStreamWriter.Close();
            if (LogFileStream != null)
                LogFileStream.Close();
        }
    }
}
