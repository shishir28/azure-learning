az login

az telemetry --enable

$resourceGroup = "shishiraddressbookrg"
$location = "australiaeast"
$webApp = "shishiraddressbook"


az webapp identity assign --resource-group $resourceGroup --name $webApp

$principalId = az webapp identity  show --resource-group $resourceGroup --name $webApp --query ['principalId'] --output tsv
$dbServer = "shishiraddressbookdbserver" 
$displayName = "msiadmin" 


az sql server ad-admin create --resource-group $resourceGroup --server-name $dbServer --display-name $displayName --object-id $principalId