az  login


$resourceGroup = "apimanagementrg"
$location = "australiaeast"


# Create resource group
az  group create -n $resourceGroup -l $location

$webApp = "shishirtaskapi"



