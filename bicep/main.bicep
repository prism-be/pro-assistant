param location string = resourceGroup().location
param application string = resourceGroup().name

param storageAccountName string = '${application}str'
param cosmosDbName string = '${application}cdb'
param logAnalyticsName string = '${application}log'
param insightsName string = '${application}ins'
param serviceBusName string = '${application}bus'
param containerAppEnvironmentName string = '${application}cev'
param containerAppWebName string = '${application}web'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  kind: 'Storage'
  properties: {
    encryption: {
      services: {
        blob: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    supportsHttpsTrafficOnly: true
  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-03-01-preview' = {
  name: cosmosDbName
  location: location
  kind: 'MongoDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    backupPolicy: {
      type: 'Continuous'
      continuousModeProperties: {
        tier: 'Continuous7Days'
      }
    }
    capabilities: [
      {
        name: 'EnableMongo'
      }
      {
        name: 'DisableRateLimitingResponses'
      }
      {
        name: 'EnableServerless'
      }
    ]
    apiProperties: {
      serverVersion: '4.2'
    }
    enableFreeTier: false
    capacity: {
      totalThroughputLimit: 4000
    }
  }
}

resource logWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    Request_Source: 'IbizaAIExtension'
    WorkspaceResourceId: logWorkspace.id
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Basic'
  }
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusAuthroization 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2022-10-01-preview' = {
  parent: serviceBus
  name: 'web'
  properties: {
    rights: [
      'Send'
      'Listen'
    ]
  }
}


// resource containerAppEnvironemnt 'Microsoft.App/managedEnvironments@2022-11-01-preview' = {
//   name: containerAppEnvironmentName
//   location: location
//   properties: {
//     appLogsConfiguration: {
//       destination: 'LogAnalytics'
//       logAnalyticsConfiguration: {
//         customerId: logWorkspace.properties.customerId
//         sharedKey: listKeys(logWorkspace.id, '2020-08-01').primarySharedKey
//       }
//     }
//   }
// }

// resource containerAppWeb 'Microsoft.App/containerApps@2022-11-01-preview' = {
//   name: containerAppWebName
//   location: location
//   properties: {
//     environmentId: containerAppEnvironemnt.id
//     configuration: {

//     }
//   }

// }
