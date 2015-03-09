using System;
using System.Collections.Generic;
using System.Net;

namespace MTLWB.HxS
{
    /// <summary>
    /// Summary description for Janitor
    /// </summary>
    /// 
    internal class Janitor
    {
        static bool s_currentlyExecuting = false;

        /// <summary>
        /// This method is called every 10 seconds on a thread pool thread to perform periodic tasks.
        /// </summary>
        /// <param name="stateInfo">not used.</param>
        public static void Wakeup(object stateInfo)
        {
            // Guard to keep us from reentrancy on multiple thread pool threads (if we are really slow,
            // or debugging, for example).
            if (s_currentlyExecuting)
            {
                return;
            }

            s_currentlyExecuting = true;

            // Replace the current dictionary with a new one.
            Dictionary<long, int> tmpDictionary;
            lock (Statics.DenialOfServiceDictionary)
            {
                tmpDictionary = Statics.DenialOfServiceDictionary;
                Statics.DenialOfServiceDictionary = new Dictionary<long, int>();
            }

            // Replace the previous dictionary with the saved current one.
            lock (Statics.PreviousDenialOfServiceDictionary)
            {
                Statics.PreviousDenialOfServiceDictionary = tmpDictionary;
            }

            s_currentlyExecuting = false;
        }
    }
}
