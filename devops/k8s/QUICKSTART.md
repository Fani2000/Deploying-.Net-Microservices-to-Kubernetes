# Quick Start Guide

## Step 1: Start Your Kubernetes Cluster

### Option A: Minikube (Recommended for local development)

```bash
# Start minikube
minikube start

# Verify it's running
minikube status
kubectl cluster-info
```

### Option B: Docker Desktop

1. Open Docker Desktop
2. Go to Settings → Kubernetes
3. Enable Kubernetes
4. Click "Apply & Restart"
5. Wait for Kubernetes to start

### Option C: Other Kubernetes Cluster

Ensure your `kubectl` is configured to connect to your cluster:
```bash
kubectl cluster-info
kubectl get nodes
```

## Step 2: Install nginx-ingress

```bash
# Using Helm (recommended)
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm install ingress-nginx ingress-nginx/ingress-nginx

# OR for Minikube
minikube addons enable ingress
```

## Step 3: Deploy the Stack

### Using the deployment script:

**Linux/Mac:**
```bash
cd devops/k8s
./deploy.sh
```

**Windows (PowerShell):**
```powershell
cd devops/k8s
.\deploy.ps1
```

### Or deploy manually:

```bash
cd devops/k8s

# Deploy in order
helm install db-configs ./db-configs --namespace default --create-namespace
helm install web-api ./web-api --namespace default
helm install web-client ./web-client --namespace default
```

## Step 4: Configure Local Access

Add these entries to your hosts file:

**Linux/Mac:** `/etc/hosts`
**Windows:** `C:\Windows\System32\drivers\etc\hosts`

```
127.0.0.1 api.local
127.0.0.1 web.local
```

## Step 5: Access Services

### For Minikube:
```bash
# Run in a separate terminal (keeps running)
minikube tunnel
```

Then access:
- Web: http://web.local
- API: http://api.local

### For Docker Desktop:
Services should be accessible directly via localhost.

### For other clusters:
Get the ingress IP:
```bash
kubectl get ingress
```

Then use the IP address instead of 127.0.0.1 in your hosts file.

## Verify Deployment

```bash
# Check all pods are running
kubectl get pods

# Check services
kubectl get svc

# Check ingress
kubectl get ingress

# View logs if issues
kubectl logs <pod-name>
```

## Troubleshooting

If you see connection errors, ensure:
1. ✅ Kubernetes cluster is running (`kubectl cluster-info`)
2. ✅ nginx-ingress is installed and running
3. ✅ For minikube: `minikube tunnel` is running
4. ✅ Hosts file entries are correct

For more details, see [README.md](README.md)

