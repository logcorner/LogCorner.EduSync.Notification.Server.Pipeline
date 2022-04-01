https://kubernetes.docker.com/hub-notification-server

az acr login --name locornermsacrtest
docker tag logcornerhub/logcorner-edusync-notification-server locornermsacrtest.azurecr.io/logcorner-edusync-notification-server
docker push locornermsacrtest.azurecr.io/logcorner-edusync-notification-server


kubectl rollout restart deployment hub-notification-server-deployment -n qa