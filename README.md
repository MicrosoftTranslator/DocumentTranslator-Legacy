# Microsoft Document Translator
The Microsoft Document Translator translates Microsoft Office, plain text, HTML, PDF files and SRT caption files, from and to any of the 70+ languages supported by the Microsoft Translator web service.
Document Translator uses the customer's own credentials and subscription to perform the Translation. Document Translator also may use custom MT systems trained via Custom Translator (https://portal.customtranslator.azure.ai).
Document Translator uses Version 3 of the Translator API. 

## Quickstart
To use the Document Translator app release to translate your documents:
1.	Download the [latest release of Document Translator on GitHub](https://github.com/MicrosoftTranslator/DocumentTranslator/releases)
2.	[Sign up](https://www.microsoft.com/en-us/translator/business/trial/#get-started) for a subscription to the Microsoft Translator Text API
3.	Enter your Translator Text API subscription key in the Settings menu
4.	Translate your documents

## Overview
Translate one or more Office documents, plain text HTML or PDF documents to another language, in one go. 

## Purpose
- Translate an Office document and receive a translated Office document in full fidelity, as an Office document. The translated Office document is fully editable like any normal Office document.
- Translate a text-based PDF document and receive a translated document in Microsoft Word.
- Translate HTML or plain text files and receive translated HTML or plain text.

Document Translator does not translate images embedded in a document. It will retain them as is. 

## Key Features
- Enter account credentials
- Define a Custom Translator-customized system to use vi its ID (optional)
- Select the files to translate. You can select multiple files at once. 
- Choose from and any number of to languages. Specifying a "From"-language is optional, in that case the system auto-detects the language.
- Document Translator creates translated files in the same folder as the original, with a name like originalname.language.docx
- Can be used via command line or graphical user interface

## Usage
Runs on Windows 7 and above.
Requires .Net Framework 4.5.
The Release is an MSI package, install directly in Windows.

- Start Microsoft Document Translator from the Start Menu.
- Visit the settings page and follow the links to subscribe to Microsoft Translator. Free for up to 2 million characters per month.
- On the settings page, follow the link to obtain your API key, and copy the key to the settings page.
- Go to the document translation page and select the documents to translate.
- Select the from and to languages.
- Hit Go.

### Command line operation
Document Translator can be run from the command line:

Translate documents:

`DocumentTranslatorCmd translatedocuments /documents:d:\testdocuments\*.docx /from:en /to:de,el`

*When a wildcard is given, Document Translator recurses through subdirectories.


Set credentials:

`DocumentTranslatorCmd setcredentials /APIkey:AzureKey /Region:westeurope /Cloud:Global /categoryID:your customization category ID`

Delete stored credentials:

`DocumentTranslatorCmd setcredentials /reset`



## How to build Document Translator
Microsoft Document Translator is written in C#, compiled in Visual Studio 2017.

It depends on

- First Floor MUI
- Microsoft.Practices.Prism for the messaging and interfaces
- OpenXml for the Office document handling
- Wix Toolkit for the installer
- HTML Agility Pack
- Newtonsoft JSON

*You need to install these separately if you want to build.


## Third party notices

This project uses:

### First Floor MUI

(c) First Floor Software

Under the Ms-PL: https://github.com/firstfloorsoftware/mui/blob/master/LICENSE.md

Available from: https://github.com/firstfloorsoftware/mui

### Html Agility Pack

Under the Ms-PL: : https://github.com/zzzprojects/html-agility-pack/blob/master/LICENSE 

Available from: https://github.com/zzzprojects/html-agility-pack 


## Security
All requests to the Translator service are SSL encrypted, using the certificate of the Microsoft Translator service.
Document Translator stores the Azure Key unencypted in the
user profile on the machine. For enterprise use we recommend to implement a more secure storage mechanism.


## Questions and Support
For questions and support please turn to the Microsoft Translator developer forum: 

https://social.msdn.microsoft.com/Forums/en-US/home?category=translation

## Enhancements
Please branch and contribute back your enhancements. Especially interested in additional file formats.

## Code of Conduct
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
