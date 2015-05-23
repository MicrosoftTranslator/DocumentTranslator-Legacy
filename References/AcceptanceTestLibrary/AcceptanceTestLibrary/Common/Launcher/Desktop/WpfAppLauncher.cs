//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Automation;
using System.Threading;
using AcceptanceTestLibrary.Common;
using AcceptanceTestLibrary.ApplicationHelper;
using AcceptanceTestLibrary.ApplicationObserver;
using System.IO;

namespace AcceptanceTestLibrary.Common.Desktop
{
    public class WpfAppLauncher : AppLauncherBase, IStateObserver
    {
        private static Process targetProcess;

        public override List<AutomationElement> LaunchApp(string applicationName, string processName)
        {
            ApplicationProcessName = processName;
            StateDiagnosis.Instance.StartDiagnosis(this);
            List<AutomationElement> aeList = new List<AutomationElement>();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(applicationName);
                startInfo.WorkingDirectory = Path.GetDirectoryName(applicationName);
                startInfo.UseShellExecute = false;

                targetProcess = Process.Start(startInfo);
                Thread.Sleep(12000);

                AutomationElement aeWindow = AutomationElement.FromHandle(targetProcess.MainWindowHandle);
                StateDiagnosis.Instance.StopDiagnosis(this);
                aeList.Add(aeWindow);
                return aeList;
            }
            catch (Exception)
            {
                UnloadApp(targetProcess);
                return null;
            }
        }

        public static Process GetCurrentAppProcess()
        {
            return Process.GetProcesses().First<Process>(proc => proc.ProcessName.Equals(ApplicationProcessName));
        }

        public static string ApplicationProcessName
        { get; set; }

        public override void UnloadApp(Process p)
        {
            p.Kill();
            p.Dispose();
        }

        public override void UnloadApp()
        {
            targetProcess.Kill();
            targetProcess.Dispose();
        }

        #region IStateObserver Members

        public void Notify()
        {
            UnloadApp(GetCurrentAppProcess());
        }

        #endregion
    }
}
