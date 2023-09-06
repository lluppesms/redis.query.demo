# To deploy this main.bicep manually:
# az login
# az account set --subscription <subscriptionId>

$resourceGroupName='rg-graphquery-dev'
$appName='lll-graphquery'
$deploymentName='manual-main-20230825T1533Z'
$runDateTime='20230825-1533' 
az deployment group create -n $deploymentName --resource-group $resourceGroupName --template-file 'main.bicep' --parameters appName=$appName environmentCode='dev' location='eastus' keyVaultOwnerUserId='af35198e-8dc7-4a2e-a41e-b2ba79bebd51' storageSku='Standard_LRS' functionAppSku='Y1' functionAppSkuFamily='Y' functionAppSkuTier='Dynamic' runDateTime=$runDateTime

# Just the functionapp
$resourceGroupName='rg-graphquery-dev'
$deploymentName='manual-function-20230825T1533Z'
$functionAppName='lll-graphquery-dev' 
$functionAppServicePlanName='lll-graphquery-dev-appsvc' 
$functionInsightsName='lll-graphquery-dev-insights' 
$functionStorageAccountName='lllgraphquerydevstrfunc3' 
$workspaceId='/subscriptions/a0f86c93-146a-4534-b83e-49090394aa78/resourceGroups/rg-graphquery-dev/providers/Microsoft.OperationalInsights/workspaces/lll-graphquery-dev-logworkspace' 
az deployment group create -n $deploymentName --resource-group $resourceGroupName --template-file 'functionapp.bicep' --parameters functionAppName=$functionAppName functionAppServicePlanName=$functionAppServicePlanName functionInsightsName=$functionInsightsName location='eastus' appInsightsLocation='eastus' functionKind='functionapp,linux' functionAppSku='Y1' functionAppSkuFamily='Y' functionAppSkuTier='Dynamic' functionStorageAccountName=$functionStorageAccountName workspaceId=$workspaceId