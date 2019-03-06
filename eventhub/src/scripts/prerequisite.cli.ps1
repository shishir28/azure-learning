az login


$resourceGroup = "MonadEventhubRG"
$location = "australiaeast"
$eventHubNamespace = "monadeventhubns"
$eventHubName = "monadeventhub"

$storageAccName = "monadeventhubstrg"
$storageSKU = "Standard_LRS"

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU


# Create resource group
az group create -n $resourceGroup -l $location

# Create an Event Hubs namespace. Specify a name for the Event Hubs namespace.
az eventhubs namespace create --name  $eventHubNamespace --resource-group $resourceGroup -l $location

# Create an event hub. Specify a name for the event hub. 

az eventhubs eventhub create --name $eventHubName --resource-group $resourceGroup --namespace-name $eventHubNamespace

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU


az group delete -n $resourceGroup -y

