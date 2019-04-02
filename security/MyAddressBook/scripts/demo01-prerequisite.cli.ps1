az login

az telemetry --enable

$resourceGroup = "ShishirAddressBookRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$dbAdminPassword = "Test1234#"
$dbAdminUser = "sqladmin"
$dbServer = "shishiraddressbookdbserver"
$dbName = "shishiraddressbookdb"




# create a DB Server

az sql server create   --name $dbServer --admin-user $dbAdminUser  --admin-password $dbAdminPassword  --location  $location  --resource-group  $resourceGroup 
 
# create a DB 

az sql db create --name $dbName --resource-group $resourceGroup  --server $dbServer

# create a Storage Account 
$storageAccName = "shishiraddressstrg"
$storageSKU = "Standard_LRS"
$container = "photos"
$storageAccKey = "1kFxuSRZjt//92lienheMsXHqedd0JCVZqmvCu+Pfj5EaXRD/NvcCLuVQsNBwLjOvwryN3zWX6Ov+NGR0vpaGQ=="



az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU

az storage container create    --account-name $storageAccName    --account-key $storageAccKey    --name $container   --public-access blob

$redisName = "shishiraddressbook"
$redisSKU = "Basic"
$redisVMSize = "C1"

# create a Redis Cache 
az redis create  --name $redisName  --resource-group $resourceGroup --location $location  --sku $redisSKU --vm-size $redisVMSize


# create a new Azure Key Vault

$kvName = "shishiraddressbookKV2"

az keyvault create --name $kvName --resource-group $resourceGroup  --location $location



# convert our secret value to a "secure string"
# following line is causing error . Set this value manually for the time being in portal 
#$secretvalue = ConvertTo-SecureString -String "shishiraddressbook.redis.cache.windows.net:6380,password=2Wm+o3J+0jpmPbn+KHNTt4C0TCqFp6n4fCoDsq15lok=,ssl=True,abortConnect=False" -AsPlainText -Force


# add the secure string to our new Key Vault

$secretName = 'CacheConnection'

# create a secret
#$secret = az keyvault secret set --vault-name $kvName --name $secretName  --value $secretvalue

# register application in KV

# create a secret policy

az keyvault set-policy --name $kvName --secret-permissions get  --spn 9287cda0-43a5-4ba9-bf79-1ae8f3ab6c84


az group delete -n $resourceGroup -y

