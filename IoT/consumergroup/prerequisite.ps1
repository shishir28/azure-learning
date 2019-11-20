az login --use-device-code


$resourceGroup = "Monad-IoT-Hub-RG"
$location = "australiaeast"
$iotHubName = "monad-iot-hub"
$storageSKU = "Standard_RAGRS"
$storageAccName = "monadiothubstrg" 
$device_container = "device-ids"
$dbServer = "monadiothubdbserver"
$dbName = "monadiothubdb"

$firewallRule="escavoxOfficeIP"
$firewallStartIpAddress="49.255.223.000"
$firewallEndIpAddress="49.255.223.255"


# create Storage Account

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU --kind StorageV2
$storageAccKey =  az storage account keys list --resource-group $resourceGroup --account-name $storageAccName --query [0].value
az storage container create --account-name $storageAccName --account-key $storageAccKey --name $device_container   --public-access blob


# create iot hub

az iot hub create  --resource-group $resourceGroup  --name $iotHubName

# create a DB Server

az sql server create   --name $dbServer --admin-user $dbAdminUser  --admin-password $dbAdminPassword  --location  $location  --resource-group  $resourceGroup 
 
# create a DB 

az sql db create --name $dbName --resource-group $resourceGroup  --server $dbServer

#Set firewall rule

az sql server firewall-rule create -g $resourceGroup -s $dbServerr -n $firewallRule --start-ip-address $firewallStartIpAddress --end-ip-address $firewallEndIpAddress


echo '{"Application_Type":"web"}' > props.json



$monad_func_app = "monad-iot-hub-func"
$monad_func_plan = "monad-iot-hub-plan"
$monad_func_app_insights = "monad-iot-hub-insights"

# create  function app
az resource create -g $resourceGroup -n $monad_func_app_insights --resource-type "Microsoft.Insights/components" --properties "@props.json"
az resource create   -g $resourceGroup  --name $monad_func_plan  --resource-type Microsoft.web/serverfarms   --is-full-object   --properties "@consumptionplan-props.json"
az functionapp create  -n $router_func_app  -g $resourceGroup  --storage-account $storageAccName  -p $router_func_plan   --app-insights $monad_func_app_insights   --runtime dotnet 


# Execute following line to do clean up. 
# az group delete -n $resourceGroup -y

