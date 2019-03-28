az login

az telemetry --enable

$resourceGroup = "MonadADRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$userName = "johndoe@shishirm.onmicrosoft.com"
$userDisplayName = "John Doe"
$password = "Davo6163"

# Create ad user

az ad user create --user-principal-name $userName --display-name $userDisplayName --password $password  


#following command shoul run from above user login account

$appName = "shishirwebapp"
$homepageURL = "http://localhost:5000"
$signonURL = "http://localhost:5000/signin-oidc"
$signoutURL = "http://localhost:5000/signout-oidc"

$nativeAppFlag = 'false'


az ad app create --display-name $appName  --homepage $homepageURL --native-app $nativeAppFlag  --identifier-uris $signonURL 



$appName = "shishirwebapp"
$homepageURL = "http://localhost:5000"
$signonURL = "http://localhost:5000/signin-oidc"
$signoutURL = "http://localhost:5000/signout-oidc"

$nativeAppFlag = 'false'


az ad app create --display-name $appName  --homepage $homepageURL --native-app $nativeAppFlag  --identifier-uris $signonURL 


# az group delete -n $resourceGroup -y

 