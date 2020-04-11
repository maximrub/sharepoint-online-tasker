# SharePointOnlineTasker

SharePointOnlineTasker is .NET Core console application which performs tasks (sets of actions) on files stored in the connected Office 365 group.

## Prerequisites

### Create a new App registrations.
Careate a new App registrations in Azure's Acitve Directory, and add 'http://localhost' to it's Redirect URIs.

### Configure the takser.

fill the info for fowwilowing fields in appsettings.json:
- AppRegistration:TenantId
- AppRegistration:ClientId
- GroupName
- DriveName
- LocalRoot

## Getting Started

Just add an implementation of the IDriveFileTask interface per operation, and it will be auto discoverd and executed per file.
