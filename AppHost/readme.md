## Aspire documentaion
https://aspire.dev/get-started/first-app/?lang=csharp

### Generating k8s manifests
cd .\AppHost<br>
dotnet tool install -g aspirate<br>
aspirate generate

### Deploying to k8s

provide files<br>
aspirate-output/apiservice/.apiservice.secrets<br>
PLACEHOLDER=1<br><br>
aspirate-output/sqlserver/.sqlserver.secrets<br>
MSSQL_SA_PASSWORD=<YOUR_STRONG_SQL_PASSWORD><br><br>
kubectl apply -k .\aspirate-output

#### cluster restart (e.g. after changes)
kubectl rollout restart deployment/apiservice

##### Minikube
https://minikube.sigs.k8s.io/docs/start/?arch=%2Fwindows%2Fx86-64%2Fstable%2F.exe+download

##### Access MS SQL
kubectl port-forward svc/sqlserver 1433:1433
<br>
ssms: 127.0.0.1,1433

##### Access API
kubectl port-forward svc/apiservice 8080:8080<br>
http://127.0.0.1:8080/swagger/index.html

##### to do after api code changes
###### ('swaggerfix' is a sample tag to make sure the new image is used)
docker build --no-cache -t apiservice:swaggerfix -f .\ApiService\Dockerfile .<br>
minikube image load apiservice:swaggerfix<br>
kubectl set image deployment/apiservice apiservice=apiservice:swaggerfix<br>
kubectl rollout status deployment/apiservice<br>