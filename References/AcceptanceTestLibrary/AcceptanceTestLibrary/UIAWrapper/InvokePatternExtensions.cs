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
    public static class InvokePatternExtensions
    {
        public static void Click(this AutomationElement Control)
        {
                //check if the buttonControl is indeed a handle to a button-based control
                InvokePattern invPattern = ValidateButtonControl(Control);

                //click the button control
                invPattern.Invoke();                 
         
        }

        private static InvokePattern ValidateButtonControl(AutomationElement element)
        {
            object invPattern = null;
            bool isValid = element.TryGetCurrentPattern(InvokePattern.Pattern,out invPattern);

            if (isValid)
            {
                return (InvokePattern)invPattern;
            }
            else
            {
                throw new InvalidOperationException("Invalid operation");
            }
        }
    }
}
