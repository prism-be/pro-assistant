param location string = resourceGroup().location
param application string = resourceGroup().name

param storageAccountName string = '${application}str'
param cosmosDbName string = '${application}cdb'
param logAnalyticsName string = '${application}log'
param insightsName string = '${application}ins'
param serviceBusName string = '${application}bus'
param containerAppEnvironmentName string = '${application}cev'
param containerAppWebName string = '${application}web'

param containerAppCertificateName string = 'cloudflare'
param webCustomDomain string = 'web.pro-assistant.eu'
param imageVersion string = '5.11.0'

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

var storageKeys = storageAccount.listKeys()
#disable-next-line no-hardcoded-env-urls
var storageConnectionString = format('DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net', storageAccount.name, storageKeys.keys[0].value)

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

resource serviceBusQueueUpdateAccountingForecast 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~accounting-forecast'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateAccountingReportingPeriod 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~accounting-reporting-period'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateAppointments 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~appointments'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateContacts 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~contacts'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateDocumentsConfiguration 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~documents-configuration'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateSettings 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~settings'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusQueueUpdateTariffs 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBus
  name: 'domain~events~tariffs'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}

resource serviceBusAuthorization 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2022-10-01-preview' = {
  parent: serviceBus
  name: 'web'
  properties: {
    rights: [
      'Send'
      'Listen'
    ]
  }
}

resource containerAppEnvironnement 'Microsoft.App/managedEnvironments@2022-11-01-preview' = {
  name: containerAppEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logWorkspace.properties.customerId
        sharedKey: logWorkspace.listKeys().primarySharedKey
      }
    }
  }
}

resource certificate 'Microsoft.App/managedEnvironments/certificates@2022-11-01-preview' existing = {
  name: containerAppCertificateName
  parent: containerAppEnvironnement
}

resource containerAppWeb 'Microsoft.App/containerApps@2022-11-01-preview' = {
  name: containerAppWebName
  location: location
  properties: {
    environmentId: containerAppEnvironnement.id
    configuration: {
      secrets: [
        {
          name: 'insights'
          value: insights.properties.ConnectionString
        }
        {
          name: 'mongo'
          value: cosmosDb.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'servicebus'
          value: serviceBusAuthorization.listKeys().primaryConnectionString
        }
        {
          name: 'storage'
          value: storageConnectionString
        }
      ]
      ingress: {
        external: true
        targetPort: 80
        customDomains: [
          {
            name: webCustomDomain
            bindingType: 'SniEnabled'
            certificateId: certificate.id
          }
        ]
      }
    }
    template: {
      containers: [
        {
          name: 'web'
          image: format('ghcr.io/prism-be/pro-assistant:{0}',imageVersion)
          resources: {
            cpu: json('0.25')
            memory: '.5Gi'
          }
          env: [
            {
              name: 'AZURE_AD_AUTHORITY'
              value: 'https://byprism.b2clogin.com/byprism.onmicrosoft.com/B2C_1_PRO_ASSISTANT/v2.0/'
            }
            {
              name: 'AZURE_AD_CLIENT_ID'
              value: 'b210005a-b610-43e2-9dd5-824e50b9f692'
            }
            {
              name: 'AZURE_AD_TENANT_ID'
              value: '220d6f01-1195-4f61-b59d-83046933e9b7'
            }
            {
              name: 'AZURE_AD_USER_FLOW'
              value: 'B2C_1_PRO_ASSISTANT'
            }
            {
              name: 'AZURE_AD_TENANT_NAME'
              value: 'byprism'
            }
            {
              name: 'ENVIRONMENT'
              value: resourceGroup().name
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              secretRef: 'insights'
            }
            {
              name: 'MONGODB_CONNECTION_STRING'
              secretRef: 'mongo'
            }
            {
              name: 'AZURE_STORAGE_CONNECTION_STRING'
              secretRef: 'storage'
            }
            {
              name: 'AZURE_SERVICE_BUS_CONNECTION_STRING'
              secretRef: 'servicebus'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 0
      }
    }
  }
}
