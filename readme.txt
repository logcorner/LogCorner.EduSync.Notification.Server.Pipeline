update dns
ipconfig /flushdns
https://kubernetes.agic.com/hub-notification-server/logcornerhub




install ingress for doecker desktop
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.3/deploy/static/provider/aws/deploy.yaml

kubectl get pods -n ingress-nginx --watch

C:\Windows\System32\drivers\etc\hosts

127.0.0.1 kubernetes.docker.com

curl http://kubernetes.docker.com
https://kubernetes.docker.com/speech-command-http-api/swagger/index.html


eneable ssl
https://slproweb.com/products/Win32OpenSSL.html


openssl req -x509 -nodes -days 365 -newkey rsa:2048 -out logcorner-ingress-tls.crt -keyout logcorner-ingress-tls.key -subj "/CN=kubernetes.docker.com/O=logcorner-ingress-tls"

kubectl create namespace qa
kubectl create secret tls logcorner-ingress-tls --namespace qa --key logcorner-ingress-tls.key --cert logcorner-ingress-tls.crt



https://kubernetes.docker.com/hub-notification-server/logcornerhub

sonarqube

dotnet sonarscanner begin /k:"LogCorner.EduSync.Notification.Server.Pipeline" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="c0c5185e46364857299f1a98c6af4b4554fc73fa"
dotnet sonarscanner end /d:sonar.login="c0c5185e46364857299f1a98c6af4b4554fc73fa"

