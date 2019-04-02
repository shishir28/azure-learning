az login

$resourceGroup = "shishiraddressbookrg"
$location = "australiaeast"
$storageAccName = "shishiraddressstrg"



#To use customer-managed keys with SSE, you must assign a storage account identity to the storage account. 


az storage account update --assign-identity  --name $storageAccName --resource-group $resourceGroup

$principalId = az storage account  show --name $storageAccName --resource-group $resourceGroup --query ['identity.principalId'] --output tsv

$kvName = "shishiraddressbookkv2"
$resouceType = 'Microsoft.KeyVault/vaults'

#Enable soft delete
#az resource update --resource-group $resourceGroup  --name $kvName --resource-type $resouceType --set properties.enableSoftDelete=true

#enable "Do Not Purge" on a key vault
#az resource update --resource-group $resourceGroup  --name $kvName --resource-type $resouceType --set properties.enablePurgeProtection=true

#associate the above key with an existing storage account using the following PowerShell commands
$customerManagedKey = 'custermanagedkey01'
$kvName = 'shishiraddressbookkv2'

$key =  az keyvault key create  --name $customerManagedKey  --vault-name $kvName --protection software

# create a  policy



az  keyvault set-policy --name $kvName --object-id $principalId --key-permissions wrapkey unwrapkey get



$kv_uri=$(az keyvault show -n $($kvName) -g  $($resourceGroup) --query properties.vaultUri -o tsv)
#$key_version=$(az keyvault key list-versions -n $customerManagedKey --vault-name $kvName --query [].kid -o tsv | cut -d '/' -f 6)

$key_version=  '6fb8e6f5413b43cea32ec7341f1671e1' #$(az keyvault key list-versions -n 'custermanagedkey01' --vault-name 'shishiraddressbookkv1' --query [].kid -o tsv | cut -d '/' -f 6)



az storage account update  --name $storageAccName  --resource-group $resourceGroup  --encryption-key-name $customerManagedKey  --encryption-key-version $key_version  --encryption-key-source Microsoft.Keyvault --encryption-key-vault $kv_uri


