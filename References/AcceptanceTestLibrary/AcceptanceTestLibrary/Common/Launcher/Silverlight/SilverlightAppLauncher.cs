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
using System.IO;
using System.Threading;
using AcceptanceTestLibrary.ApplicationHelper;
using AcceptanceTestLibrary.Common;
using AcceptanceTestLibrary.ApplicationObserver;

namespace AcceptanceTestLibrary.Common.Silverlight
{
    public class SilverlightAppLauncher : AppLauncherBase, IStateObserver
    {
        /// <summary>
        /// This method launches silverlight application
        /// </summary>
        /// <param name="applicationPath">Silverlight Application path</param>
        /// <param name="browserTitle">Browser Title</param>
        /// <returns></returns>
        public override List<AutomationElement> LaunchApp(string applicationPath, string browserTitle)
        {
            this.ApplicationTitle = browserTitle;
            StateDiagnosis.Instance.StartDiagnosis(this);
            try
            {
                List<AutomationElement> aeList = BrowserSupportHandler.LaunchBrowser(applicationPath, this.ApplicationTitle);
                StateDiagnosis.Instance.StopDiagnosis(this);

                return aeList;
            }

            catch (Exception)
            {
                StateDiagnosis.Instance.StopDiagnosis(this);
                return null;
            }
            
        }

        public override void UnloadApp(Process p)
        {
            if (p!= null && !p.HasExited)
            {
                p.Kill();
            }
            p.Dispose();
        }

        public override void UnloadApp()
        {
        }
        public static void UnloadBrowser(string applicationTitle)
        {
            BrowserSupportHandler.UnloadBrowser(applicationTitle);
        }

        public string ApplicationTitle
        {
            get;
            set;
        }

        #region IStateObserver Members

        public void Notify()
        {
            UnloadBrowser(this.ApplicationTitle);
        }

        #endregion
    }
}
