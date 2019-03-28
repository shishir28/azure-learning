az login

az telemetry --enable

$resourceGroup = "MonadADRG"
$location = "australiaeast"

# Create resource group
az group create -n $resourceGroup -l $location

$userName = "johndoe@monadsystems.onmicrosoft.com"
$userDisplayName = "John Doe"
$password = "*********"

# Create ad user

az ad user create --user-principal-name $userName --display-name $userDisplayName --password $password  


#following command should run from above user login account


$appName = "shishirwebapp"
$homepageURL = "http://localhost:5000"
$signonURL = "http://localhost:5000/signin-oidc"
#$signoutURL = "http://localhost:5000/signout-oidc"
$nativeAppFlag = 'false'


az ad app create --display-name $appName  --homepage $homepageURL --native-app $nativeAppFlag  --identifier-uris $signonURL 

#following command used to create this project
dotnet new mvc --auth  SingleOrg   --client-id 6a630468-db24-4b9e-9406-340a0603411b --tenant-id monadsystems.onmicrosoft.com --domain monadsystems.onmicrosoft.com

# az group delete -n $resourceGroup -y

 