# Azure DevOps Deployment Template Notes

## 1. Azure DevOps Template Definitions

Typically, you would want to set up the either first option or the second and third option, but not all three jobs.

- **infra-and-code-pipeline.yml:** Deploys the main.bicep template, builds the function app, then deploys the function app to the Azure Function
- **infra-only-pipeline.yml:** Deploys the main.bicep template and does nothing else
- **app-only-pipeline.yml:** Builds the function app and then deploys the function app to the Azure Function
- **test-harness-build-pipeline.yml:** Builds the console utility that can be used to test the app and then copies the EXE file to a storage account for easy access.

---

## 2. Deploy Environments

These Azure DevOps YML files were designed to run as multi-stage environment deploys (i.e. DEV/QA/PROD). Each Azure DevOps environments can have permissions and approvals defined. For example, DEV can be published upon change, and QA/PROD environments can require an approval before any changes are made.

---

## 3. Setup Steps

- [Create Azure DevOps Service Connections](https://docs.luppes.com/CreateServiceConnections/)

- [Create Azure DevOps Environments](https://docs.luppes.com/CreateDevOpsEnvironments/)

- Create Azure DevOps Variable Groups - see next step in this document (the variables are unique to this project)

- [Create Azure DevOps Pipeline(s)](https://docs.luppes.com/CreateNewPipeline/)

- [Deploy the Azure Resources and Application](./Docs/DeployApplication.md)

---

## 4. Creating the variable group "GraphQuery"

To create this variable groups, customize and run this command in the Azure Cloud Shell.

``` bash
   az login

   az pipelines variable-group create 
     --organization=https://dev.azure.com/<yourAzDOOrg>/ 
     --project='<yourAzDOProject>' 
     --name GraphQuery 
     --variables 
         orgName='<yourInitials>-GraphQuery' 
         appSuffix=''
         environmentCode='dev' 
         serviceConnectionName='<yourServiceConnection>' 
         azureSubscription='<yourAzureSubscriptionName>' 
         subscriptionId='<yourSubscriptionId>' 
         location='eastus' 
         storageSku='Standard_LRS' 
         functionAppSku='Y1' 
         functionAppSkuFamily='Y' 
         functionAppSkuTier='Dynamic' 
         keyVaultOwnerUserId='owner1SID'
```

---

## 5. Running the Application

[How to run the application](../Docs/RunApplication.md)

---

[Reference: Using Azurite Local Storage](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&tabs=visual-studio)
