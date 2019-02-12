az login

$resourceGroup = "ShishirAciGhostDemoRG"
$location = "australiaeast"

az group create -n $resourceGroup -l $location

$containerGroupName = "shishirghost-blog1"
az container create `
    -g $resourceGroup -n $containerGroupName `
    --image ghost `
    --ports 2368 `
    --ip-address public `
    --dns-name-label shishirghostaci 

az container show  -g $resourceGroup -n $containerGroupName

# gets the domain name

$fqdn = az container show `
    -g $resourceGroup `
    -n $containerGroupName `
    --query ipAddress.fqdn `
    -o tsv

$site = "http://$($fqdn):2368"

Start-Process $site

# visit the blog admin page
Start-Process "$site/ghost"

az container logs `
    -n $containerGroupName -g $resourceGroup 

az group delete -n $resourceGroup -y
