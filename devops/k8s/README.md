# Kubernetes Helm Charts Deployment Guide

This directory contains Helm charts for deploying the microservices stack to Kubernetes.

## Prerequisites

1. Kubernetes cluster (local or remote) - **MUST BE RUNNING**
2. Helm 3.x installed
3. kubectl configured to access your cluster
4. nginx-ingress controller installed

### Starting Your Kubernetes Cluster

**For Minikube:**
```bash
minikube start
```

**For Docker Desktop:**
- Open Docker Desktop
- Go to Settings → Kubernetes
- Enable Kubernetes
- Click "Apply & Restart"

**Verify cluster is running:**
```bash
kubectl cluster-info
kubectl get nodes
```

## Installing nginx-ingress

For local development (e.g., minikube, kind, or Docker Desktop):

```bash
# Using Helm
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm install ingress-nginx ingress-nginx/ingress-nginx

# Or using kubectl (for minikube)
minikube addons enable ingress

# Or for Docker Desktop, enable ingress in settings
```

## Deployment Order

Deploy the charts in the following order to ensure dependencies are met:

### 1. Deploy MongoDB (db-configs)

```bash
cd devops/k8s/db-configs
helm install db-configs . --namespace default --create-namespace
```

### 2. Deploy API Service (web-api)

```bash
cd devops/k8s/web-api
helm install web-api . --namespace default
```

### 3. Deploy Web Client (web-client)

```bash
cd devops/k8s/web-client
helm install web-client . --namespace default
```

## Quick Deploy Script

You can also deploy all services at once:

```bash
#!/bin/bash
cd devops/k8s

helm install db-configs ./db-configs --namespace default --create-namespace
helm install web-api ./web-api --namespace default
helm install web-client ./web-client --namespace default
```

## Accessing Services

After deployment, configure your `/etc/hosts` file (or equivalent) to map the ingress hosts:

```
127.0.0.1 api.local
127.0.0.1 web.local
127.0.0.1 prometheus.local
127.0.0.1 grafana.local
```

**Note:** For local clusters, you may need to get the ingress IP:

```bash
# For minikube
minikube tunnel

# For Docker Desktop, services are accessible via localhost
# For other clusters, get the ingress IP:
kubectl get ingress
```

### Service URLs

- **Web Client**: http://web.local
- **API Service**: http://api.local

## Updating Services

To update a service:

```bash
helm upgrade <release-name> ./<chart-directory> --namespace default
```

## Uninstalling

To remove all services:

```bash
helm uninstall db-configs web-api web-client --namespace default
```

## Configuration

Each chart has a `values.yaml` file where you can customize:

- Resource limits and requests
- Replica counts
- Environment variables
- Ingress settings
- Storage configurations

## Troubleshooting

### Cluster Connection Issues

**Error: "dial tcp 127.0.0.1:XXXXX: connectex: No connection could be made"**

This means your Kubernetes cluster is not running. Start it:

```bash
# For Minikube
minikube start

# Verify it's running
minikube status
kubectl cluster-info
```

**Error: "error validating data: failed to download openapi"**

This is usually a cluster connectivity issue. Try:
1. Ensure your cluster is running (see above)
2. Verify kubectl context: `kubectl config current-context`
3. Test connection: `kubectl get nodes`

### Check pod status:
```bash
kubectl get pods
```

### Check service endpoints:
```bash
kubectl get svc
```

### Check ingress:
```bash
kubectl get ingress
```

### View logs:
```bash
kubectl logs <pod-name>
```

### Describe pod for events:
```bash
kubectl describe pod <pod-name>
```

### Common Issues

**Pods stuck in Pending:**
- Check resource availability: `kubectl describe pod <pod-name>`
- Check if PVCs are bound: `kubectl get pvc`

**Ingress not working:**
- Ensure nginx-ingress is installed and running
- For minikube: run `minikube tunnel` in a separate terminal
- Check ingress controller: `kubectl get pods -n ingress-nginx`

## Service Dependencies

- **web-api** depends on **db-configs** (MongoDB)
- **web-client** depends on **web-api**

Make sure to deploy in the order specified above to avoid startup issues.

