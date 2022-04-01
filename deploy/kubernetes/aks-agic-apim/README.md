# LogCorner.EduSync.Notification.Server

az acr login --name acrlogcorner

kubectl create secret docker-registry regsecret --docker-server=acrlogcorner.azurecr.io --docker-username=acrlogcorner  --docker-password=6eQtd1P+x70yfXMENj8GhpWB4GIcjRWf  --docker-email=admin@azurecr.io  

docker tag  logcornerhub/logcorner-edusync-signalr-server  acrlogcorner.azurecr.io/logcorner-edusync-signalr-server 
docker push acrlogcorner.azurecr.io/logcorner-edusync-signalr-server 


kubectl get secret regsecret --output=yaml
kubectl get secret regcred --output="jsonpath={.data.\.dockerconfigjson}" | base64 --decode




command ==> 

docker tag  logcornerhub/logcorner-edusync-speech-command   acrlogcorner.azurecr.io/logcorner-edusync-speech-command 
docker push acrlogcorner.azurecr.io/logcorner-edusync-speech-command 



docker tag  logcornerhub/logcorner-edusync-speech-command-data   acrlogcorner.azurecr.io/logcorner-edusync-speech-command-data 
docker push acrlogcorner.azurecr.io/logcorner-edusync-speech-command-data 

query ==>
docker tag  logcornerhub/logcorner-edusync-speech-query    acrlogcorner.azurecr.io/logcorner-edusync-speech-query
docker push acrlogcorner.azurecr.io/logcorner-edusync-speech-query



front ==>
docker tag  logcornerhub/logcorner-edusync-speech-query    acrlogcorner.azurecr.io/logcorner-edusync-speech-query
docker push acrlogcorner.azurecr.io/logcorner-edusync-speech-query*



kubectl create secret tls agic-ingress-tls --namespace default --key agic-ingress-tls.key --cert agic-ingress-tls.crt

51.137.7.145 kubernetes.agic.com
ipconfig/flushdns  cmd
https://kubernetes.agic.com/hub-notification-server/logcornerhub

{
  "title": "this is the title",
  "description": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.",
  "url": "http://test.com",
  "typeId": 1
}