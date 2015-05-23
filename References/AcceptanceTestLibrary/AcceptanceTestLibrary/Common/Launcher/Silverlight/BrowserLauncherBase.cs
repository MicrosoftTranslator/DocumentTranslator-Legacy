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
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;
using AcceptanceTestLibrary.ApplicationHelper;

namespace AcceptanceTestLibrary.Common.Launcher.Silverlight
{
    public abstract class BrowserLauncherBase : IBrowserLauncher
    {
        public virtual AutomationElement LaunchBrowser(string silverlightApplicationPath, string applicationTitle)
        {
            try
            {
                //This method will contain the implementation for launching the silverlight application
                ApplicationTitleInBrowser = applicationTitle;

                Process appProcess = Process.Start(GetBrowserPath(), silverlightApplicationPath);
                ProcessId = appProcess.Id;

                Thread.Sleep(15000);

                //Get the current process and return that
                Process targetProcess = GetCurrentAppProcess();

                if (!(targetProcess.HasExited || targetProcess.MainWindowHandle == IntPtr.Zero))
                {
                    return (AutomationElement.FromHandle(targetProcess.MainWindowHandle));
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual Process GetCurrentAppProcess()
        {
            try
            {
                //Finding out process based on the title is a temporary design. Code will be enhanced to get the process using other options.
                return Process.GetProcesses().First<Process>(proc =>
                    proc.ProcessName.Equals(ConfigHandler.GetValue(GetBrowserProcessName())) &&
                        proc.MainWindowTitle.StartsWith(ApplicationTitleInBrowser));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual string ApplicationTitleInBrowser
        { get; set; }

        public abstract string GetBrowserPath();

        public abstract string GetBrowserProcessName();

        internal int ProcessId
        {
            get;
            set;
        }
    }
}
