# Provisioning
# 1. Build the device 
# 2. Configure and assign the identity or extarct 

# 3. Solution operator with device manufacturer so that device identity is put into some central registry
# a. Provide API manufacturer to push registration information 
#  b. issue certificate manufacture can assign to device as they are created 

#4. Provisioning : When device starts , contact centra registry and prove its identity , 
# registry provides the connection information for device to connect to IoT platform (IoT hub)
# AZ IoT hub device provisioning service (DPS)   does all this 

# The way DPS works 
    #a. Entollment: Enroll a device or enrol group 
    #b. device first time  , connects to DPs and proves identity 
    #c, DPS register the device to hub 

    # DPS : Zero-tocuh deployment , Load-balancing , Geo-sharding , Tenant isolation

    # Security : Attestation (Authentication) : 3 Methods 
        #a. Trusted Platform module : Most complicated and secured . Individual enrollment
        #b. Symmetric Key : Easy to get up and running not considered very secure . Individual enrollment
        #c. X.509 Certs : Middle path. Also supports group enrollment. Store the certs on AZ KV 
        




