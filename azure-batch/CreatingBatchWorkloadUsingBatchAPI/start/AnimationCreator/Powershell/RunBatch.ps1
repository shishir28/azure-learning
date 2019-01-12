
Param(
	[string] $subscriptionId = '',
	[string] $Location = 'Australia East',
	[string] $ResourceGroupName = 'PSBatchRG',
	[string] $BatchAccountName = 'shishirpsbatchaccount',
	[string] $PoolName = 'RenderPool',
	[string] $LowPriorityPoolName = 'LowPriorityRenderPool'
)

# Authenticate the PowerShell session with the Azure API
Login-AzureRmAccount

# Select the right subscription 
Select-AzureRmSubscription -SubscriptionID $subscriptionId

# create new resource group
New-AzureRmResourceGroup `
    -Name $ResourceGroupName `
    -Location $Location `
    -Force

# Create a new Batch Account
New-AzureRmBatchAccount `
    –AccountName $BatchAccountName `
    –Location $Location `
    –ResourceGroupName $ResourceGroupName

# Retrieve the Batch Account authentication credentials
$context = Get-AzureRmBatchAccountKeys `
    -AccountName $BatchAccountName

# Create a Cloud Service Configuration
$configuration = New-Object  `
    -TypeName "Microsoft.Azure.Commands.Batch.Models.PSCloudServiceConfiguration"  `
    -ArgumentList @(4,"*")


# Create a Pool
New-AzureBatchPool `
    -Id $PoolName  `
    -VirtualMachineSize "Small"  `
    -CloudServiceConfiguration $configuration  `
    -TargetDedicatedComputeNodes 1 `
    -BatchContext $context

# Create a Low Priority Pool
New-AzureBatchPool `
    -Id $LowPriorityPoolName  `
    -VirtualMachineSize "Small"  `
    -CloudServiceConfiguration $configuration  `
    -TargetLowPriorityComputeNodes 1 `
    -BatchContext $context

# Remove the Batch Account

Remove-AzureRmBatchAccount -AccountName $BatchAccountName -Force