// --------------------------------------------------------------------------------
// Bicep file that builds all the resource names used by other Bicep templates
// --------------------------------------------------------------------------------
param appName string = ''
@allowed(['azd','gha','azdo','dev','demo','qa','stg','ct','prod'])
param environmentCode string = 'azd'

param functionStorageNameSuffix string = 'func'
param dataStorageNameSuffix string = 'data'

// --------------------------------------------------------------------------------
var lowerAppName = replace(toLower(appName), ' ', '')
var sanitizedAppName = replace(replace(lowerAppName, '-', ''), '_', '')
var sanitizedEnvironment = toLower(environmentCode)

// pull resource abbreviations from a common JSON file
var resourceAbbreviations = loadJsonContent('./resourceAbbreviations.json')

// --------------------------------------------------------------------------------
// other resource names can be changed if desired, but if using the "azd deploy" command it expects the
// function name to be exactly "{appName}function" so don't change the functionAppName format if using azd
var functionAppName = environmentCode == 'azd' ? '${lowerAppName}${resourceAbbreviations.functionSuffix}' : toLower('${lowerAppName}-${sanitizedEnvironment}')
var baseStorageName = toLower('${sanitizedAppName}${sanitizedEnvironment}${resourceAbbreviations.storageAccountSuffix}')

// --------------------------------------------------------------------------------
output logAnalyticsWorkspaceName string =  toLower('${lowerAppName}-${sanitizedEnvironment}-${resourceAbbreviations.logWorkspaceSuffix}')
output functionAppName string            = functionAppName
output functionAppServicePlanName string = '${functionAppName}-${resourceAbbreviations.appServicePlanSuffix}'
output functionInsightsName string       = '${functionAppName}-${resourceAbbreviations.appInsightsSuffix}'
output redisCacheName string             = '${functionAppName}-${resourceAbbreviations.redisSuffix}'

// Key Vaults and Storage Accounts can only be 24 characters long
output keyVaultName string               = take(toLower('${sanitizedAppName}${sanitizedEnvironment}${resourceAbbreviations.keyVaultAbbreviation}'), 24)
output functionStorageName string        = take('${baseStorageName}${functionStorageNameSuffix}', 24)
output dataStorageName string            = take('${baseStorageName}${dataStorageNameSuffix}', 24)
