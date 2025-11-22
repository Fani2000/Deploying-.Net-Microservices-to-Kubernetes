# Docker Compose - Microservices Architecture

This docker-compose.yaml file replicates the Aspire application architecture for local development.

## Architecture Overview

```
┌─────────────────┐
│   MongoDB       │
│   (Database)    │
└────────┬────────┘
         │
         │ Connection: mongodb://mongo:27017
         │
┌────────▼────────┐
│  ApiService     │
│  (.NET 9)       │
│  - Products API │
│  - Weather API  │
└────────┬────────┘
         │
         │ HTTP: http://apiservice:8080
         │ (via ProductApiClient & WeatherApiClient)
         │
┌────────▼────────┐
│  WebFrontend    │
│  (.NET 8 Blazor)│
│  - Web UI       │
└─────────────────┘
```

## Service Connections

### Aspire Configuration (src.AppHost/Program.cs)
- **MongoDB**: `builder.AddMongoDB("mongo").AddDatabase("mongodb")`
- **ApiService**: `builder.AddProject<Projects.src_ApiService>("apiservice").WithReference(mongodb)`
- **WebFrontend**: `builder.AddProject<Projects.src_Web>("webfrontend").WithReference(apiService)`

### Docker Compose Mapping
- **MongoDB** → `mongo` service (port 27017)
- **ApiService** → `apiservice` service (port 8080)
- **WebFrontend** → `webfrontend` service (port 8081)

## Service Details

### MongoDB
- **Image**: mongo:7.0
- **Port**: 27017
- **Database**: ProductDB
- **Volume**: Persistent storage for data
- **Health Check**: MongoDB ping command

### ApiService
- **Framework**: .NET 9
- **Port**: 8080 (mapped to host)
- **Endpoints**:
  - `/api/products` - Product CRUD operations
  - `/api/products/{id}` - Get product by ID
  - `/api/products/category/{category}` - Get products by category
  - `/weatherforecast` - Weather forecast data
  - `/swagger` - Swagger UI (development)
- **Dependencies**: MongoDB
- **Features**:
  - Automatic data seeding on startup
  - Swagger/OpenAPI documentation
  - Health checks

### WebFrontend
- **Framework**: .NET 8 Blazor Server
- **Port**: 8081 (mapped to host)
- **Connections**:
  - `ProductApiClient` → `http://apiservice:8080`
  - `WeatherApiClient` → `http://apiservice:8080`
- **Dependencies**: ApiService
- **Features**:
  - Blazor Server interactive components
  - Product listing page
  - Weather forecast page

## Service Discovery

### In Aspire
- Uses built-in service discovery: `"https+http://apiservice"`
- Automatically resolves service endpoints
- Handles HTTPS/HTTP preference

### In Docker Compose
- Uses Docker service names for resolution
- Direct HTTP connection: `http://apiservice:8080`
- Configured via environment variables

## Usage

### Start all services
```bash
docker-compose up -d
```

### View logs
```bash
docker-compose logs -f
```

### Stop all services
```bash
docker-compose down
```

### Stop and remove volumes
```bash
docker-compose down -v
```

## Access Points

- **Web Frontend**: http://localhost:8081
- **API Service**: http://localhost:8080
- **API Swagger**: http://localhost:8080/swagger (development)
- **MongoDB**: localhost:27017
- **Grafana Dashboard**: http://localhost:3000 (admin/admin) - Observability & Monitoring
- **Prometheus**: http://localhost:9090 - Metrics query and storage
- **OTEL Collector Metrics**: http://localhost:8889/metrics - Prometheus format metrics

## Environment Variables

### ApiService
- `ConnectionStrings__mongodb`: MongoDB connection string
- `MongoDB__DatabaseName`: Database name (default: ProductDB)

### WebFrontend
- `Services__apiservice`: ApiService base URL (overrides Aspire service discovery)

## Data Seeding

The ApiService automatically seeds MongoDB with initial product data on first startup:
- 6 sample products
- Seeding is idempotent (won't duplicate on restarts)
- Data persists in MongoDB volume

## Network

All services are connected via `microservices-network` bridge network, allowing them to communicate using service names.

## Monitoring & Observability

### Architecture

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐
│ ApiService  │────▶│ OTEL         │────▶│ Aspire          │
│ WebFrontend │     │ Collector    │     │ Dashboard       │
└─────────────┘     └──────────────┘     └─────────────────┘
     │                    │                        │
     │ OTLP (gRPC)        │ OTLP (gRPC)            │
     │ Port 4317          │ Port 18888             │ UI Port 15000
     └────────────────────┴────────────────────────┘
```

### Components

1. **OpenTelemetry Collector** (`otel-collector`)
   - Receives telemetry data (metrics, traces, logs) from all services via OTLP
   - Forwards data to Aspire Dashboard
   - Exposes Prometheus metrics endpoint
   - Ports: 4317 (gRPC), 4318 (HTTP), 8889 (Prometheus)

2. **Aspire Dashboard** (`aspire-dashboard`)
   - Provides observability dashboard similar to Aspire
   - Displays metrics, traces, logs, and service health
   - Port: 15000 (Dashboard UI)
   - OTLP endpoints: 18888 (gRPC), 18889 (HTTP)

### Telemetry Flow

1. **Services** (ApiService, WebFrontend) export telemetry via OpenTelemetry OTLP exporter
2. **OTEL Collector** receives telemetry on port 4317 (gRPC)
3. **OTEL Collector** processes and forwards to Aspire Dashboard on port 18888
4. **Aspire Dashboard** visualizes metrics, traces, and logs

### Environment Variables for Telemetry

Both ApiService and WebFrontend are configured with:
- `OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317` - OTLP exporter endpoint
- `OTEL_SERVICE_NAME` - Service name for identification
- `OTEL_RESOURCE_ATTRIBUTES` - Additional resource metadata

### What You Can Monitor

- **Metrics**: Request rates, response times, error rates, CPU/Memory usage
- **Traces**: Distributed tracing across services (via OTEL Collector logs)
- **Logs**: Application logs with structured data (via OTEL Collector)
- **Health**: Service health status and dependencies
- **Service Performance**: Real-time metrics visualization in Grafana

### Setting Up Grafana Dashboards

1. Access Grafana at http://localhost:3000
2. Login with admin/admin
3. Prometheus datasource is pre-configured
4. Create dashboards or import pre-built .NET/AspNetCore dashboards
5. Query metrics using PromQL

### Example PromQL Queries

- Request rate: `rate(http_server_request_duration_seconds_count[5m])`
- Error rate: `rate(http_server_request_duration_seconds_count{status_code=~"5.."}[5m])`
- Response time: `histogram_quantile(0.95, http_server_request_duration_seconds_bucket)`

