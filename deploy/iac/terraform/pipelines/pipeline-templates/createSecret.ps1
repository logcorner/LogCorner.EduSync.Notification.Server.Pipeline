Import-AzAksCredential -ResourceGroupName LOGCORNER-MICROSERVICES-IAC -Name demo-apim-aks-test -Force
$secretDeployed =  $false
$secretName='agic-ingress-tls'
$namespace ='default'
$secretKey ='agic-ingress-tls.key'
$secretCert ='agic-ingress-tls.crt'
try {
    $secret =  kubectl get secret  $secretName  -o json  | ConvertFrom-Json  
    if ($secret.metadata.name = $secretName) {
        $secretDeployed  = $true
        Write-Host "secret $secretName found  $secretDeployed"
    }
    else {
        $secretDeployed =  $false
        Write-Host "secret $secretName not found $secretDeployed"
    }
}
catch {
    Write-Host "secret $secretName not found $secretDeployed"
    $secretDeployed =  $false
}
if (-not $secretDeployed ){
    kubectl create secret tls $secretName --namespace $namespace --key $secretKey --cert $secretCert
}