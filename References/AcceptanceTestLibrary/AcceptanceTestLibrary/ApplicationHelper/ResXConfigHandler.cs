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
using System.Resources;
using System.Collections;
using System.Globalization;
using AcceptanceTestLibrary.Common;

namespace AcceptanceTestLibrary.ApplicationHelper
{
    /// <summary>
    /// Handler to read value from XML file (having key-value pair) on specifying the XML file path and the key.
    /// </summary>
    public class ResXConfigHandler : IDisposable
    {
        ResXResourceReader rsxr;

        public ResXConfigHandler(string filePath)
        {
            // Create a ResXResourceReader for the file items.resx.
            rsxr = new ResXResourceReader(filePath);

            // Create an IDictionaryEnumerator to iterate through the resources.
            rsxr.GetEnumerator();
        }

        public virtual string GetValue(string key)
        {
            // Iterate through the resources and display the contents to the console.
            foreach (DictionaryEntry d in rsxr)
            {
                if (d.Key.ToString().Equals(key))
                {
                    return String.Format(CultureInfo.InvariantCulture, d.Value.ToString());
                }
            }
            return String.Empty;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != rsxr)
                {
                    //Close the reader.
                    rsxr.Close();
                }
            }
        }
    }
}
