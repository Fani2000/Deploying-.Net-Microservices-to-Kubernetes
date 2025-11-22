# Deployment script for Kubernetes microservices stack (PowerShell)
# This script deploys all Helm charts in the correct order

param(
    [string]$Namespace = "default"
)

Write-Host "Deploying microservices stack to namespace: $Namespace" -ForegroundColor Green

# Check if kubectl can connect to cluster
Write-Host "Checking Kubernetes cluster connection..." -ForegroundColor Yellow
$clusterInfo = kubectl cluster-info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Cannot connect to Kubernetes cluster!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please ensure your Kubernetes cluster is running:" -ForegroundColor Yellow
    Write-Host "  - For minikube: run 'minikube start'" -ForegroundColor Yellow
    Write-Host "  - For Docker Desktop: enable Kubernetes in settings" -ForegroundColor Yellow
    Write-Host "  - For other clusters: ensure kubectl is configured correctly" -ForegroundColor Yellow
    exit 1
}

Write-Host "Cluster connection verified ✓" -ForegroundColor Green

# Create namespace if it doesn't exist
kubectl create namespace $Namespace --dry-run=client -o yaml | kubectl apply -f -

# Deploy MongoDB
Write-Host "Deploying MongoDB..." -ForegroundColor Yellow
helm upgrade --install db-configs ./db-configs --namespace $Namespace

# Wait for MongoDB to be ready
Write-Host "Waiting for MongoDB to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=db-configs -n $Namespace --timeout=300s

# Deploy API Service
Write-Host "Deploying API Service..." -ForegroundColor Yellow
helm upgrade --install web-api ./web-api --namespace $Namespace

# Wait for API Service to be ready
Write-Host "Waiting for API Service to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=web-api -n $Namespace --timeout=300s

# Deploy Web Client
Write-Host "Deploying Web Client..." -ForegroundColor Yellow
helm upgrade --install web-client ./web-client --namespace $Namespace

Write-Host ""
Write-Host "Deployment complete!" -ForegroundColor Green
Write-Host ""
Write-Host "To access services, add these to your C:\Windows\System32\drivers\etc\hosts file:" -ForegroundColor Cyan
Write-Host "127.0.0.1 api.local"
Write-Host "127.0.0.1 web.local"
Write-Host ""
Write-Host "For local clusters, you may need to:" -ForegroundColor Cyan
Write-Host "  - Run: minikube tunnel"
Write-Host "  - Or enable ingress in Docker Desktop settings"
Write-Host ""
Write-Host "Check status with:" -ForegroundColor Cyan
Write-Host "  kubectl get pods -n $Namespace"
Write-Host "  kubectl get ingress -n $Namespace"

