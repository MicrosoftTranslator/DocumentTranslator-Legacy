// // ----------------------------------------------------------------------
// // <copyright file="SolutionModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>SolutionModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business.Model
{
    #region

    using System.Collections.Generic;

    #endregion

    public class SolutionModel
    {
        #region Public Properties

        public Dictionary<string, List<string>> Projects { get; set; }

        public string Solution { get; set; }

        #endregion
    }
}