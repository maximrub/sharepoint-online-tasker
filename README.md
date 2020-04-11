+# GoogleDriveTasker

GoogleDriveTasker is .NET Core console application which performs tasks (sets of actions) on files stored in the connected Google Drive.

## Prerequisites

### Turn on the Drive API

#### Create a new Cloud Platform project and enable the Drive API.
https://developers.google.com/drive/api/v3/enable-drive-api

#### Configure an OAuth client.

Any application that uses OAuth 2.0 to access Google APIs must have authorization credentials that identify the application to Google's OAuth 2.0 server.
The following steps explain how to create credentials for your project. Your applications can then use the credentials to access APIs that you have enabled for that project.

Go to the [Credentials page](https://console.developers.google.com/apis/credentials).
Click Create credentials > OAuth client ID.
Select "Other" for Application type.
Download the client configuration and save the file as credentials.json in your working directory.

## Getting Started

Just add an implementation of the IGoogleDriveFileTask interface per operation, and it will be auto discoverd and executed per file.
