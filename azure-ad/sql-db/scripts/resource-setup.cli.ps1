az login

az telemetry --enable

$resourceGroup = "MonadADRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$dbAdminPassword = "Test1234#"
$dbAdminUser = "sqladmin"
$dbServer = "monadaddbserver"
$dbName = "monadaddb"


# create a DB Server

az sql server create   --name $dbServer --admin-user $dbAdminUser  --admin-password $dbAdminPassword  --location  $location  --resource-group  $resourceGroup 
 
# create a DB 

az sql db create --name $dbName --resource-group $resourceGroup  --server $dbServer


# az group delete -n $resourceGroup -y

 