// // ----------------------------------------------------------------------
// // <copyright file="CommentTranslationModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>CommentTranslationModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business.Model
{
    public class CommentTranslationModel
    {
        #region Public Properties

        public string SourceLanguage { get; set; }

        public string TargetLanguage { get; set; }

        public string TargetPath { get; set; }

        #endregion
    }
}