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

namespace AcceptanceTestLibrary.UIAWrapper
{
    public static class ExpandCollapsePatternExtensions
    {
        public static void Expand(this AutomationElement expandControl)
        {
            //check if the buttonControl is indeed a handle to a button-based control
            ExpandCollapsePattern expColPattern = ValidateExpandCollapseControl(expandControl);

            //expand
            expColPattern.Expand();
        }

        public static void Collapse(this AutomationElement expandControl)
        {
            //check if the buttonControl is indeed a handle to a button-based control
            ExpandCollapsePattern expColPattern = ValidateExpandCollapseControl(expandControl);

            //Collapse
            expColPattern.Collapse();
        }

        private static ExpandCollapsePattern ValidateExpandCollapseControl(AutomationElement element)
        {
            object expColPattern = null;
            bool isValid = element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out expColPattern);

            if (isValid)
            {
                return (ExpandCollapsePattern)expColPattern;
            }
            else
            {
                throw new InvalidOperationException("Invalid operation");
            }
        }
    }
}
