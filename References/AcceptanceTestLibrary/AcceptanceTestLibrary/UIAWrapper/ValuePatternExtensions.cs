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
    public static class ValuePatternExtensions
    {
        public static string GetValue(this AutomationElement textControl)
        {
            //check if the textControl is indeed a handle to a text-based control
            ValuePattern valPattern = ValidateTextControl(textControl);

            //get value from the texbox
            return valPattern.Current.Value;
        }

        public static void SetValue(this AutomationElement textControl, string value)
        {
            //check if the textControl is indeed a handle to a text-based control
            ValuePattern valPattern = ValidateTextControl(textControl);

            valPattern.SetValue(value);
        }

        private static ValuePattern ValidateTextControl(AutomationElement element)
        {
            object valPattern = null;
            bool isValid = element.TryGetCurrentPattern(ValuePattern.Pattern,out valPattern);

            if (isValid)
            {
                return (ValuePattern)valPattern;
            }
            else
            {
                throw new InvalidOperationException("Invalid operation");
            }
        }
    }
}
