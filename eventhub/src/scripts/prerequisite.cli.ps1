az login --use-device-code


$resourceGroup = "MonadEventhubRG"
$location = "australiaeast"
$eventHubNamespace = "monadeventhubns"
$eventHubName = "monadeventhub"

$storageAccName = "monadeventhubstrg"
$storageSKU = "Standard_LRS"

$dbAdminPassword = "Test1234#"
$dbAdminUser = "sqladmin"
$dbServer = "monadeventhubdbserver"
$dbName = "monadeventhubdb"




# Create resource group
az group create -n $resourceGroup -l $location

# Create an Event Hubs namespace. Specify a name for the Event Hubs namespace.
az eventhubs namespace create --name  $eventHubNamespace --resource-group $resourceGroup -l $location

# Create an event hub. Specify a name for the event hub. 

az eventhubs eventhub create --name $eventHubName --resource-group $resourceGroup --namespace-name $eventHubNamespace

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU


# create a DB Server

az sql server create   --name $dbServer --admin-user $dbAdminUser  --admin-password $dbAdminPassword  --location  $location  --resource-group  $resourceGroup 
 
# create a DB 

az sql db create --name $dbName --resource-group $resourceGroup  --server $dbServer



# Execute following line to do clean up. 
# az group delete -n $resourceGroup -y

