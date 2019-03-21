az login

az telemetry --enable

$resourceGroup = "ShishirNotesRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$appPlanName = "shishirnotesplan"
$webAppName = "shishirnoteswebapp"
$webApiName = "shishirnoteswebapi"

$eventHubNamespace = "shishirnoteseventhubns"
$eventHubName = "shishirnoteseventhub"

$storageAccName = "shishirnotesstrg"
$storageSKU = "Standard_LRS"
$container = "notes-events"



# Create app service plan 

az appservice plan create --name  $appPlanName  -g $resourceGroup  --sku S1

# create a web application 
az webapp create -g $resourceGroup -p $appPlanName   -n $webAppName

az webapp create -g $resourceGroup -p $appPlanName   -n $webApiName


az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU

$storageAccKey =  az storage account keys list --resource-group $resourceGroup --account-name $storageAccName --query [0].value


az storage container create    --account-name $storageAccName    --account-key $storageAccKey    --name $container   --public-access blob


# Create an Event Hubs namespace. Specify a name for the Event Hubs namespace.
az eventhubs namespace create --name  $eventHubNamespace --resource-group $resourceGroup -l $location

# Create an event hub. Specify a name for the event hub. 

az eventhubs eventhub create --name $eventHubName --resource-group $resourceGroup --namespace-name $eventHubNamespace

# create CosmosDB
$cosmosDBName = "shishirnotescosmosdb"

az cosmosdb create --name $cosmosDBName --resource-group $resourceGroup


$redisName = "shishirnotes"
$redisSKU = "Basic"
$redisVMSize = "C1"

# create a Redis Cache 
az redis create  --name $redisName  --resource-group $resourceGroup --location $location  --sku $redisSKU --vm-size $redisVMSize


$vmName = "shishirnotesVM"
$vmAdminUser = "shishiradmin"
$vmAdminPassword = "Welcome1234#"


# create a VM

az vm create -n $vmName -g $resourceGroup  --admin-username  $vmAdminUser  --admin-password  $vmAdminPassword   --location $location   --image Win2012R2Datacenter 

$appInsightsName = "shishirnotes-appinsights"



$propsFile = "props.json"
'{"Application_Type":"web"}' | Out-File $propsFile


az resource create  -n $appInsightsName -g $resourceGroup  --resource-type "Microsoft.Insights/components"  --properties "@$propsFile"

# az group delete -n $resourceGroup -y

