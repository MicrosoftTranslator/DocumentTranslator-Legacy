using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTLWB.Common.Log
{
    internal class ConsoleLogger : LogBase
    {
        internal ConsoleLogger(LogType[] levels)
            : base(levels)
        {
        }

        internal override void LogInternal(LogEntry entry)
        {
            Console.WriteLine(entry.ToConsoleString());
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
