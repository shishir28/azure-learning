$resourceGroup = "MonadIoTRG"
$location = "australiaeast"
$hubName = "monadDemoHub"
$deviceName = "monadDevice1"


$storageAccName = "monadiotstrg"
$storageSKU = "Standard_LRS"


# Create resource group
az group create -n $resourceGroup -l $location

# Create a IoT Hub 

# Create resource group
az iot hub create --resource-group $resourceGroup -n $hubName --sku S1  -l $location --partition-count 4

# Create a device
az iot hub device-identity create --device-id $deviceName --hub-name $hubName 


# Send D2C Message
az iot device send-d2c-message -n $hubName  -d $deviceName --data 'Hello from cli'

# monitor 
az iot hub monitor-events -n $hubName  --resource-group $resourceGroup

# Send C2D Message

az iot device c2d-message send -n $hubName  -d $deviceName --data 'Hello from clod 2 device'

# Receive C2D Message
az iot device c2d-message receive -n $hubName  -d $deviceName 


#simulating device with interval as well
az iot device simulate -n $hubName -d $deviceName --data "Mesaging from simulated device"  --msg-count 5 --msg-interval 1

#create storage account

az storage account create  --location $location  --name $storageAccName  --resource-group  $resourceGroup  --sku $storageSKU

az iot hub routing-endpoint create --resource-group $resourceGroup  --connection-string  --hub-name $hubName  --endpoint-name  S1 --endpoint-type azurestoragecontainer   --endpoint-resource-group $resourceGroup 
                                   
# device twin    
az iot hub device-twin show --device-id $deviceName   --hub-name   $hubName          

# az group delete -n $resourceGroup -y