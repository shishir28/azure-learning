﻿# Set date range for exported usage data

$reportedStartTime = "2019-04-01"

$reportedEndTime = "2019-04-30"

# Authenticate to Azure

Login-AzureRmAccount

# Select an Azure Subscription for which to report usage data

$subscriptionId = (Get-AzureRmSubscription | Out-GridView  -Title "Select an Azure Subscription ..."    -PassThru).SubscriptionId


$subscriptionId = '8995c075-9c0c-46c1-be82-1f14ed5fdcf8'

Select-AzureRmSubscription -SubscriptionId $subscriptionId

# Set path to exported CSV file

$filename = ".\usageData-${subscriptionId}-${reportedStartTime}-${reportedEndTime}.csv"

# Set usage parameters

$granularity = "Daily" # Can be Hourly or Daily

$showDetails = $true

# Export Usage to CSV

$appendFile = $false

$continuationToken = ""

$usageData = Get-UsageAggregates `
    -ReportedStartTime $reportedStartTime `
    -ReportedEndTime $reportedEndTime `
    -AggregationGranularity $granularity `
    -ShowDetails:$showDetails 

Do { 

    $usageData.UsageAggregations.Properties | 
        Select-Object `
            UsageStartTime, `
            UsageEndTime, `
            @{n='SubscriptionId';e={$subscriptionId}}, `
            MeterCategory, `
            MeterId, `
            MeterName, `
            MeterSubCategory, `
            MeterRegion, `
            Unit, `
            Quantity, `
            @{n='Project';e={$_.InfoFields.Project}}, `
            InstanceData | 
        Export-Csv `
            -Append:$appendFile `
            -NoTypeInformation:$true `
            -Path $filename

    if ($usageData.ContinuationToken) {

        $continuationToken = $usageData.ContinuationToken

        $usageData = Get-UsageAggregates `
            -ReportedStartTime $reportedStartTime `
            -ReportedEndTime $reportedEndTime `
            -AggregationGranularity $granularity `
            -ShowDetails:$showDetails `
            -ContinuationToken $continuationToken

    } else {

        $continuationToken = ""

    }

    $appendFile = $true

} until (!$continuationToken)

