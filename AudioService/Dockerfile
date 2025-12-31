# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ["AudioService/AudioService.csproj", "AudioService/"]

# Restore dependencies
RUN dotnet restore "AudioService/AudioService.csproj"

# Copy source code
COPY ["AudioService/", "AudioService/"]

# Build and publish
WORKDIR "/src/AudioService"
RUN dotnet publish "AudioService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Copy appsettings
COPY ["AudioService/appsettings.json", "."]

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_ENVIRONMENT=Production

# Health check (optional)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD dotnet --version || exit 1

# Run the service
ENTRYPOINT ["dotnet", "AudioService.dll"]
