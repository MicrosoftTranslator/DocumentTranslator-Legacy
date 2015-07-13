# Microsoft Document Translator
The Microsoft Document Translator translates Microsoft Office, plain text, and PDF files, from any of the 50 languages supported by the Microsoft Translator web service, to any other of these 50 languages.
Document Translator uses the customer's own credentials and subscription to perform the service, and will make use of any translation stored in the collaborative translations framework,
as well as making use of customer's customized MT system.

## Overview
Translate one or more Office documents, plain text or PDF files to another language, in one go. 

## Purpose
Translate an Office document and receive a translated Office document in full fidelity, as an Office document. The translated Office document is fully editable like any normal Office document. 

## Key Features
- Enter account credentials
- Define a Hub-customized system to use (optional)
- Select the files to translate
- Choose from and to langauges. From is optional, in that case the system auto-detects the language.
- Document Translator creates translated files in the same folder as the original, with a name like originalname.language.docx

## Usage
Runs on Windows 7 and above.
Requires .Net Framework 4.5.
The Release is an MSI package, install directly in Windows. 

- Start Microsoft Document Translator from the Start Menu.
- Visit the settings page and follow the links to subscribe to Microsoft Translator. Free for up to 2 millionn characters per month.
- On the settigs page, follow the link to defining your client ID and secret, and copy them to the settings page.
- Go to the document translation page and select the documents to translate.
- Select the from and to languages.
- Hit Go.

### Command line operation
Document Translator can be run from the command line:

Translate documents:
  DocumentTranslatorCmd translatedocuments /sourcedocuments:d:\testdocuments\*.docx /targetlanguages:de,el
When a wirldcard is give, Document Translator recurses through subdirectories.


Set credentials:
DocumentTranslatorCmd setcredentials /clientid:ClientId /clientsecret:ClientSecret



## How to build Document Translator
Microsoft Document Translator is written in C#, using Visual Studio 2013.

It depends on
- Firstfloor.ModernUI for the UI
- Microsoft.Practices.Prism for the messaging and interfaces
- OpenXml for the Office document handling

The software is open to accepting new file types for translation. 

##Questions and Support
For questions and support please turn to the Microsoft Translator developer forum: https://social.msdn.microsoft.com/Forums/en-US/home?category=translation

##Enhancements
Please feel encouraged to fork and controbute back your enhancements, specifically for new file formats.
