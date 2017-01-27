# Microsoft Document Translator
The Microsoft Document Translator translates Microsoft Office, plain text, HTML, PDF files and SRT caption files, from and to any of the 60+ languages supported by the Microsoft Translator web service.
Document Translator uses the customer's own credentials and subscription to perform the Translation. Document Translator uses the translations stored in the Collaborative Translations Framework,
and makes use of a custom MT system, trained via the Microsoft Translator Hub (https://microsoft.com/translator/hub.aspx).

## Overview
Translate one or more Office documents, plain text HTML or PDF documents to another language, in one go. 

## Purpose
- Translate an Office document and receive a translated Office document in full fidelity, as an Office document. The translated Office document is fully editable like any normal Office document.
- Translate a text-based PDF document and receive a translated document in Microsoft Word.
- Translate HTML or plain text files and receive translated HTML or plain text.
- Upload an existing TMX file to Microsoft Translator's CTF store, to be used in subsequent translations. 

## Key Features
- Enter account credentials
- Define a Hub-customized system to use (optional)
- Select the files to translate
- Choose from and to languages. Specifying a "From"-language is optional, in that case the system auto-detects the language.
- Document Translator creates translated files in the same folder as the original, with a name like originalname.language.docx

## Usage
Runs on Windows 7 and above.
Requires .Net Framework 4.5.
The Release is an MSI package, install directly in Windows.

- Start Microsoft Document Translator from the Start Menu.
- Visit the settings page and follow the links to subscribe to Microsoft Translator. Free for up to 2 million characters per month.
- On the settigs page, follow the link to defining your client ID and secret, and copy them to the settings page.
- Go to the document translation page and select the documents to translate.
- Select the from and to languages.
- Hit Go.

### Command line operation
Document Translator can be run from the command line:

Translate documents:

`DocumentTranslatorCmd translatedocuments /documents:d:\testdocuments\*.docx /from:en /to:de,el`

*When a wildcard is given, Document Translator recurses through subdirectories.


Set credentials:

`DocumentTranslatorCmd setcredentials /clientid:ClientId /clientsecret:ClientSecret`

Upload a TMX to the Collaborative Translation Framework for use in subsequent translations:

`DocumentTranslatorCmd tmxtoctf /tmx:TmxFileName.tmx /from:FromLanguage /to:ToLanguage /user:UserName /rating:Rating /write:true`

*When write is not set to true, the results of the TMX parse are rendered to the screen. It is advisable to perform the
test without write set to true in order to debug the TMX content. WARNING: Use only bilingual TMX. Document Translator performs no validation of the TMX content languages.*

Get Word Alignments

`DocumentTranslatorCmd getalignments /documents:test.txt /from:en /to:fr`

Builds a CSV file containing 3 columns: source, target, and word alignment information in the format 

[[SourceTextStartIndex]:[SourceTextEndIndex]–[TgtTextStartIndex]:[TgtTextEndIndex]] *

for every word of the source. The information for each word is separated by a space, including for non-space-separated languages (scripts) like Chinese. 
Example alignment string: "0:0-7:10 1:2-11:20 3:4-0:3 3:4-4:6 5:5-21:21".

In other words, the colon separates start and end index, the dash separates the languages, and space separates the words.
One word may align with zero, one, or multiple words in the other language, and the aligned words may be non-contiguous.
When no alignment information is available, the Alignment element will be empty. No error.


## How to build Document Translator
Microsoft Document Translator is written in C#, compiled in Visual Studio 2015.

It depends on

- First Floor MUI
- Microsoft.Practices.Prism for the messaging and interfaces
- OpenXml for the Office document handling
- Wix Toolkit for the installer
- HTML Agility Pack

*You need to install these separately if you want to build.


##Third party notices

This project uses:

###First Floor MUI

(c) First Floor Software

Under the Ms-PL: https://github.com/firstfloorsoftware/mui/blob/master/LICENSE.md

Available from: https://github.com/firstfloorsoftware/mui

###Html Agility Pack

Under the Ms-PL: http://htmlagilitypack.codeplex.com/license

Available from: http://htmlagilitypack.codeplex.com/


##Security
All requests to the Translator service are SSL encrypted, using the certificate of the Microsoft Translator service.
Document Translator stores the service access credentials (client ID and client secret) unencypted in the
user profile on the machine. For enterprise use we recommend to implement a more secure storage mechanism.

##Questions and Support
For questions and support please turn to the Microsoft Translator developer forum: 

https://social.msdn.microsoft.com/Forums/en-US/home?category=translation

##Enhancements
Please branch and contribute back your enhancements. Especially interested in additional file formats.

##Code of Conduct
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
