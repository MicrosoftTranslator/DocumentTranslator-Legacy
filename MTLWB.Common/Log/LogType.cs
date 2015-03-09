using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTLWB.Common.Log
{
    /// <summary>Types of log messages</summary>
    public enum LogType
    {
        /// <summary>Status message.  Typically used to display/log the progress of a running task.</summary>
        Status,
        /// <summary>SNT tagging fixes performed by SntErrorCheckingAndFixing.</summary>
        SntFixes,
        /// <summary>All the SNT sentences that failed to translate.</summary>
        UntranslatedSnt,
        /// <summary>All the TMX TU tags that failed while processing.</summary>
        FailedTmxTags,
        /// <summary>Warning. Typically user induced error that we handle correctly.</summary>
        Warning,
        /// <summary>Error.  Serious application error occured that we did not expect.</summary>
        Error
    }

}
