# Purpose

This is an Azure Function app that automates collating Midden files and copies the outputted catalog.json file into blob storage.

## Requirements

### Midden CLI Configuration File

This script requires a `configuration.json` file to be included at the same directory level as the `MiddenCli.exe` program. The `configuration.json` file defines the Data Stores to be crawled and is required by MiddenCli. See Midden documentation for more information.

### Service Accounts for Data Stores

This script also requires various service accounts to be configured depending on the Data Store(s) that are to be crawled. At the time writting (v0.3) MiddenCli supports Azure Data Lake Gen 2 and Google Workspace Shared Drives.

### Application settings

You will need to define three secrets in your Azure Function Application > Configuration > Application settings:

* CafPublicConnString: This should be renamed. Oops. But it's the Connection String for the Azure Blob Storage where the catalog.json file will be copied into
* ContainerName: This is the container name of the blob storage where the catalog.json file will be copied into
* BlobPath: The path (and filename, so I guess you don't actually need it to be catalog.json) for the blob. For example, 'data/Midden/catalog.json`.

## Some tips

When deploying this function app we recommend using zip-deploy. This function requires the wwwroot to be writable. Running from a package, which kicks in if you right-click publish from Visual Studio, makes the wwwroot read-only.

If additional files are required for MiddenCli (such as a authentication json file for a Google service account), specify the full path for it in the `configuration.json` file. You may need to check your Function App to see the correct path, but it should be either `D:\\home\\site\\wwwroot` or `C:\\home\\site\\wwwroot`.