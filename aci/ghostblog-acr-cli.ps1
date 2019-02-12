az login

$resourceGroup = "ShishirAciPrivatRegistryDemoRG"
$location = "australiaeast"

az group create -n $resourceGroup -l $location

$acrName = "shishiracr"

az acr create -g $resourceGroup -n $acrName `
    --sku Basic --admin-enabled true

$acrPassword = az acr credential show -n $acrName `
    --query "passwords[0].value" -o tsv


 $loginServer = az acr show -n $acrName `
    --query loginServer --output tsv

docker login -u $acrName -p $acrPassword $loginServer

$image = "mystaticsite:v1"

$imageTag = "$loginServer/$image"

docker tag $image $imageTag

docker push $imageTag

az acr repository list -n $acrName --output table

$containerGroupName = "aci-acr"
az container create -g $resourceGroup `
    -n $containerGroupName `
    --image $imageTag --cpu 1 --memory 1 `
    --registry-username $loginServer `
    --registry-password $acrPassword `
    --dns-name-label "aciacr" --ports 80

# get the site address and launch in a browser
$fqdn = az container show -g $resourceGroup -n $containerGroupName `
    --query ipAddress.fqdn -o tsv
Start-Process "http://$($fqdn)"

# view the logs for our container
az container logs -n $containerGroupName -g $resourceGroup

# delete the resource group (ACR and container group)
az group delete -n $resourceGroup -y