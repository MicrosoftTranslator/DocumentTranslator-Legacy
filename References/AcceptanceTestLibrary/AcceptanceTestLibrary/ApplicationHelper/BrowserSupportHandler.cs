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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using AcceptanceTestLibrary.ApplicationHelper;
using System.Configuration;
using System.Windows.Automation;
using AcceptanceTestLibrary.Common.Launcher;
using System.Reflection;
using System.Diagnostics;
using AcceptanceTestLibrary.Common.Launcher.Silverlight;
using AcceptanceTestLibrary.Common.CrossBrowserSupport;
using System.Collections;
using System.IO;

namespace AcceptanceTestLibrary.ApplicationHelper
{
    public static class BrowserSupportHandler
    {
        static Object obj = new Object();
        static Dictionary<string, IBrowserLauncher> browserLauncherCol = new Dictionary<string,IBrowserLauncher>();
        
        //Load browsers from Config File - Config Section
        static NameValueCollection browserCollection = ConfigHandler.GetConfigSection("BrowserSupport/Browsers");
        
        public static List<AutomationElement> LaunchBrowser(string applicationPath, string applicationTitle)
        {
            List<AutomationElement> aeList = new List<AutomationElement>();
            IBrowserLauncher browserLauncher;
            
            //Load Browsers based on the Config Section
            if (browserCollection != null && browserCollection.Count > 0)
            {
                foreach (string browser in browserCollection)
                {
                    browserLauncher = GetBrowserLauncherInstance(browserCollection[browser]);
                    aeList.Add(browserLauncher.LaunchBrowser(applicationPath, applicationTitle));
                }
            }
            else //When there is no entry in the config file return null.
            {
                return null;
            }
            return aeList;
        }

        public static void UnloadBrowser(string applicationTitle)
        {
            //Close the Internet explorer browser
            IBrowserLauncher browserLauncher;
            
            //unload browsers based on the Config Section
            if (browserCollection != null && browserCollection.Count > 0)
            {
                foreach (string browser in browserCollection)
                {
                    browserLauncher = GetBrowserLauncherInstance(browserCollection[browser]);
                    browserLauncher.ApplicationTitleInBrowser = applicationTitle;
                    UnloadProcess(browserLauncher.GetCurrentAppProcess());
                }
            }
        }

        private static IBrowserLauncher GetBrowserLauncherInstance(string T)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            lock (obj)
            {
                if (!browserLauncherCol.ContainsKey(T))
                {
                    browserLauncherCol[T] = (IBrowserLauncher)asm.CreateInstance(T);
                }
            }
            return browserLauncherCol[T];
        }

        private static void UnloadProcess(Process p)
        {
            if (p != null)
            {
                if (!p.HasExited)
                {
                    p.CloseMainWindow();
                }

                p.Dispose();
                p = null;
            }
        }
    }
}
