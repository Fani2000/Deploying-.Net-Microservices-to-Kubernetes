# Use the .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file if it exists (for better restore caching)
COPY ["*.sln", "./"]
COPY ["src.Web/src.Web.csproj", "src.Web/"]
COPY ["src.ServiceDefaults/src.ServiceDefaults.csproj", "src.ServiceDefaults/"]

# Restore dependencies (this layer will be cached if dependencies don't change)
RUN dotnet restore "src.Web/src.Web.csproj"

# Copy everything else and build
COPY ["src.Web/", "src.Web/"]
COPY ["src.ServiceDefaults/", "src.ServiceDefaults/"]
WORKDIR "/src/src.Web"
RUN dotnet build "src.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build 

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src.Web"
RUN dotnet publish "src.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published app
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser /app

# Switch to non-root user
USER appuser

# Health check (using the default health endpoint from MapDefaultEndpoints)
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "src.Web.dll"]

