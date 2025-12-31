# Quick Start Guide - AudioService

## Choose Your Deployment Method

### ?? Docker Compose (Recommended - Easiest)
```bash
# 1. From project root
docker-compose up -d

# 2. View logs
docker-compose logs -f audio-service

# 3. Stop
docker-compose down
```

**Time**: 2 minutes  
**Best for**: Local development, testing, quick deployment

### ?? Docker Manual
```bash
# 1. Build
docker build -t audio-service:latest -f AudioService/Dockerfile .

# 2. Run
docker run -d --name audio-service audio-service:latest

# 3. View logs
docker logs -f audio-service

# 4. Stop
docker stop audio-service
docker rm audio-service
```

**Time**: 3 minutes  
**Best for**: Production containers, registry pushes

### ??? Console Mode (Development/Testing)
```bash
# 1. Build
dotnet build

# 2. Run with default settings
dotnet run

# 3. Or run with Production settings
dotnet run --environment Production

# 4. Press Ctrl+C to stop
```

**Time**: 1 minute  
**Best for**: Development, debugging, testing

### ?? Kubernetes
```bash
# 1. Apply ConfigMap
kubectl apply -f k8s-configmap.yaml

# 2. Apply Deployment
kubectl apply -f k8s-deployment.yaml

# 3. Check status
kubectl get pods -l app=audio-service

# 4. View logs
kubectl logs -f deployment/audio-service
```

**Time**: 10 minutes  
**Best for**: Enterprise, scaling, orchestration

See [`KUBERNETES_EXAMPLES.md`](KUBERNETES_EXAMPLES.md) for detailed K8s setup.

---

## Verify It's Working

### Docker / Docker Compose
```bash
# Check if container is running
docker ps | grep audio-service

# View logs
docker-compose logs audio-service
# or
docker logs -f audio-service

# You should see output like:
# info: AudioService.AudioProcessingService[0]
#       Audio Processing Service starting
# info: AudioService.TaskProcessor[0]
#       Starting audio processing at 2024-01-15 10:30:45
```

### Console Mode
Just watch the console output. You should see:
```
info: AudioService.AudioProcessingService[0]
      Audio Processing Service starting
info: AudioService.TaskProcessor[0]
      Processing audio...
info: AudioService.TaskProcessor[0]
      Audio processing completed at 2024-01-15 10:30:45
```

### Kubernetes
```bash
# Check pod status
kubectl get pods -l app=audio-service

# Watch logs
kubectl logs -f deployment/audio-service

# You should see the same output as console mode
```

---

## Configuration

### Change Processing Interval

Edit the appropriate appsettings file:

**Development** (1 minute intervals):
```bash
DOTNET_ENVIRONMENT=Development dotnet run
# or set DOTNET_ENVIRONMENT=Development in your container
```

**Production** (30 minute intervals):
```bash
DOTNET_ENVIRONMENT=Production dotnet run
# or set DOTNET_ENVIRONMENT=Production in your container
```

### Change Log Level

Edit `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"  // Change to Debug for verbose logging
    }
  }
}
```

---

## Troubleshooting

### Container exits immediately
```bash
# Check logs
docker logs audio-service

# Run in foreground to see errors
docker run -it audio-service:latest

# Check if image built correctly
docker images | grep audio-service
```

### Logs not appearing
```bash
# Make sure container is running
docker ps | grep audio-service

# Restart it
docker restart audio-service

# Check logs again
docker logs -f audio-service
```

### Configuration not applied
```bash
# Verify appsettings.json syntax (valid JSON)
cat appsettings.json

# Check environment variable is set
echo $DOTNET_ENVIRONMENT  # Linux
echo %DOTNET_ENVIRONMENT%  # Windows

# Restart service
docker restart audio-service
```

---

## Next Steps

1. ? Choose your deployment method above
2. ? Follow the quick start commands
3. ? Verify logs appear
4. ? Edit `TaskProcessor.cs` with your business logic
5. ? Configure `appsettings.json` as needed
6. ? Deploy to production

---

**Need more help?**
- See [`../README.md`](../README.md) for overview
- See [`CROSS_PLATFORM_DEPLOYMENT.md`](CROSS_PLATFORM_DEPLOYMENT.md) for details
- See [`QUICK_REFERENCE.md`](QUICK_REFERENCE.md) for commands
