## Aspire documentaion
https://aspire.dev/get-started/first-app/?lang=csharp

### Generating k8s manifests
cd .\AppHost<br>
dotnet tool install -g aspirate<br>
aspirate generate

### Deploying to k8s
kubectl apply -k .\aspirate-output
