#!/bin/bash

# Deployment script for Kubernetes microservices stack
# This script deploys all Helm charts in the correct order

set -e

NAMESPACE=${1:-default}

echo "Deploying microservices stack to namespace: $NAMESPACE"

# Check if kubectl can connect to cluster
echo "Checking Kubernetes cluster connection..."
if ! kubectl cluster-info &>/dev/null; then
    echo "ERROR: Cannot connect to Kubernetes cluster!" >&2
    echo "" >&2
    echo "Please ensure your Kubernetes cluster is running:" >&2
    echo "  - For minikube: run 'minikube start'" >&2
    echo "  - For Docker Desktop: enable Kubernetes in settings" >&2
    echo "  - For other clusters: ensure kubectl is configured correctly" >&2
    exit 1
fi

echo "Cluster connection verified ✓"

# Create namespace if it doesn't exist
kubectl create namespace $NAMESPACE --dry-run=client -o yaml | kubectl apply -f -

# Deploy MongoDB
echo "Deploying MongoDB..."
helm upgrade --install db-configs ./db-configs --namespace $NAMESPACE

# Wait for MongoDB to be ready
echo "Waiting for MongoDB to be ready..."
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=db-configs -n $NAMESPACE --timeout=300s || true

# Deploy API Service
echo "Deploying API Service..."
helm upgrade --install web-api ./web-api --namespace $NAMESPACE

# Wait for API Service to be ready
echo "Waiting for API Service to be ready..."
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=web-api -n $NAMESPACE --timeout=300s || true

# Deploy Web Client
echo "Deploying Web Client..."
helm upgrade --install web-client ./web-client --namespace $NAMESPACE

echo ""
echo "Deployment complete!"
echo ""
echo "To access services, add these to your /etc/hosts file:"
echo "127.0.0.1 api.local"
echo "127.0.0.1 web.local"
echo ""
echo "For local clusters, you may need to run:"
echo "  minikube tunnel"
echo "  or enable ingress in Docker Desktop settings"
echo ""
echo "Check status with:"
echo "  kubectl get pods -n $NAMESPACE"
echo "  kubectl get ingress -n $NAMESPACE"

