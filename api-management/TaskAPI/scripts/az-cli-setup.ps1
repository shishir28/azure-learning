az  login


$resourceGroup = "apimanagementrg"
$location = "australiaeast"


# Create resource group
az  group create -n $resourceGroup -l $location

$appPlanName = "shishirtaskplan"
$webApiName = "shishirtaskapi"
$webapimName = "shishirtaskapi-management"
$organizationName = "monad systems"
$adminEmail = "shishir28@live.com"

# Create app service plan 

az appservice plan create --name  $appPlanName  -g $resourceGroup  --sku S1

# create a web application 
az webapp create -g $resourceGroup -p $appPlanName   -n $webApiName

# Install-Module -Name Az.ApiManagement -Force -AllowClobber
# Import-Module -Name Az.ApiManagement
# Connect-AzAccount 


New-AzApiManagement -ResourceGroupName $resourceGroup -Location $location -Name $webapimName -Organization $organizationName -AdminEmail $adminEmail -Sku "Developer"

#az group delete -n $resourceGroup -y 
