$cert=New-SelfSignedCertificate -Subject "CN=GraphDaemonWithCert" -CertStoreLocation "Cert:\CurrentUser\My"  -KeyExportPolicy Exportable -KeySpec Signature