az login

az telemetry --enable

$resourceGroup = "ShishirAddressBookRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$dbAdminPassword = "Xyz1234#"
$dbAdminUser = "sqladmin"
$dbServer = "ShishirAddressBookDBServer"
$dbName = "ShishirAddressBook"

# create a DB Server

az sql server create   --name $dbServer --admin-user $dbAdminUser  --admin-password $dbAdminPassword  --location  $location  --resource-group  $resourceGroup 
 
# create a DB 

az sql db create --name $dbName --resource-group $resourceGroup  --server $dbServer

# create a Storage Account 
$storageAccName = "shishiraddressstrg"
$storageSKU = "Standard_LRS"

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU


# create a Container

$container = "photos"
$storageAccKey =  az storage account keys list --resource-group $resourceGroup --account-name $storageAccName --query [0].value

az storage container create    --account-name $storageAccName    --account-key $storageAccKey    --name $container   --public-access blob

$redisName = "shishiraddressbook"
$redisSKU = "Basic"
$redisVMSize = "C1"

# create a Redis Cache 
az redis create  --name $redisName  --resource-group $resourceGroup --location $location  --sku $redisSKU --vm-size $redisVMSize

$redisHostName =  az redis show --name $redisName  --resource-group $resourceGroup --query hostName --output tsv
$redisPort =  az redis show --name $redisName  --resource-group $resourceGroup --query port --output tsv
$redisPrimaryKey= az redis list-keys --name $redisName --resource-group $resourceGroup --query primaryKey --output tsv
$redisConnectionString ="$($redisHostName):$($redisPort),password=$($redisPrimaryKey),ssl=True,abortConnect=False"


# create a new Azure Key Vault

$kvName = "shishiraddressbookKV"

az keyvault create --name $kvName --resource-group $resourceGroup  --location $location


# convert our secret value to a "secure string"
$secretvalue = ConvertTo-SecureString $redisConnectionString -AsPlainText -Force

# add the secure string to our new Key Vault

$secretName = 'CacheConnection'

# create a secret
$secret = az keyvault secret set --vault-name $kvName --name $secretName  --value $secretvalue

# register application in KV


# create a secret policy

az keyvault set-policy --name $kvName --secret-permissions get  --spn fe502ad9-8d77-4b6b-83a1-0a01865f8852


# Enable soft delete

$resouceType = 'Microsoft.KeyVault/vaults'

az resource update --resource-group $resourceGroup  --name $kvName --resource-type $resouceType --set properties.enableSoftDelete=true

# remove a key vault

az keyvault delete --name $kvName --resource-group $resourceGroup

# recover a "soft deleted vault"
az keyvault recover --resource-group $resourceGroup --name $kvName --location $location


#enable "Do Not Purge" on a key vault
az resource update --resource-group $resourceGroup  --name $kvName --resource-type $resouceType --set properties.enablePurgeProtection=true

# permanantly delete a "soft deleted" key vault - does not work if "Do Not Purge" is enabled

az keyvault purge --name $kvName  --location $location

# delete resource group and every resource under it

az group delete -n $resourceGroup - y 