<# 
./createSecret.ps1 -ResourceGroupName 'LOGCORNER-MICROSERVICES-IAC' `
                     -clusterName 'demo-apim-aks-test' `
                     -namespace 'default' `
                     -secretName 'agic-ingress-tls' `
                     -secretKey 'agic-ingress-tls.key'  `
                     -secretCert 'agic-ingress-tls.crt'  
 #>

Param(
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a resource group name")]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a clusterName")]
    [string]$clusterName,
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a namespace")]
    [string]$namespace,
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a secretName")]
    [string]$secretName,
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a secretKey")]
    [string]$secretKey,
    [Parameter(Mandatory=$true, HelpMessage = "Please provide a secretCert")]
    [string]$secretCert
)

Import-AzAksCredential -ResourceGroupName $ResourceGroupName -Name $clusterName -Force
$secretDeployed =  $false
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