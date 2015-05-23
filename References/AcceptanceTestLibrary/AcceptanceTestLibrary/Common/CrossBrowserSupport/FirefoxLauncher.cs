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
using System.Threading;
using System.Diagnostics;
using AcceptanceTestLibrary.ApplicationHelper;
using AcceptanceTestLibrary.Common.Launcher;
using AcceptanceTestLibrary.Common.Launcher.Silverlight;

namespace AcceptanceTestLibrary.Common.CrossBrowserSupport
{
    public class FirefoxLauncher : BrowserLauncherBase
    {
        public override string GetBrowserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + ConfigHandler.GetValue("FirefoxPartialPath");
        }

        public override string GetBrowserProcessName()
        {
            return "FirefoxAppProcessName";
        }
    }
}
