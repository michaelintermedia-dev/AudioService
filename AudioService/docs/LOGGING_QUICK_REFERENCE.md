# Logging Quick Reference

## View Logs

### Docker Compose
```bash
# Follow logs in real-time
docker-compose logs -f audio-service

# View all logs
docker-compose logs audio-service

# Last 100 lines
docker-compose logs --tail=100 audio-service
```

### Docker
```bash
# Follow logs
docker logs -f audio-service

# View logs
docker logs audio-service

# With timestamps
docker logs --timestamps audio-service

# Last 50 lines
docker logs --tail=50 audio-service
```

### Kubernetes
```bash
# Follow logs
kubectl logs -f deployment/audio-service

# View logs
kubectl logs deployment/audio-service

# Specific pod
kubectl logs pod-name

# Last 50 lines
kubectl logs pod-name --tail=50

# Previous pod (if crashed)
kubectl logs pod-name --previous
```

### Console
```bash
# Logs appear directly in terminal output
dotnet run
```

---

## Change Log Level

### Environment Variable
```bash
# Development (verbose)
export Logging__LogLevel__Default=Debug
dotnet run

# Production (minimal)
export Logging__LogLevel__Default=Warning
dotnet run

# Windows PowerShell
$env:Logging__LogLevel__Default='Debug'
```

### Docker
```bash
# Pass environment variable
docker run \
  -e Logging__LogLevel__Default=Debug \
  audio-service:latest
```

### Kubernetes
```bash
# Update deployment
kubectl set env deployment/audio-service \
  Logging__LogLevel__Default=Debug
```

---

## Configuration Files

### Development (Verbose)
```bash
DOTNET_ENVIRONMENT=Development dotnet run
```

### Production (Minimal)
```bash
DOTNET_ENVIRONMENT=Production dotnet run
```

---

## Common Log Messages

| Message | Meaning | Level |
|---------|---------|-------|
| `===== AudioService Starting =====` | App startup | Info |
| `Environment: {env}` | Running environment | Info |
| `Processing interval configured: {min} minutes` | Service configured | Info |
| `Processing cycle #{n} starting` | Processing started | Debug |
| `----> Processing audio tasks started` | Task started | Info |
| `----> Processing audio tasks completed successfully in {ms}ms` | Task done | Info |
| `Next processing in {min} minutes` | Waiting | Info |
| `Error in Audio Processing Service (Error #{n})` | Error occurred | Error |
| `Retrying after {sec} seconds...` | Retry delay | Warning |
| `===== AudioService Stopped =====` | App shutdown | Info |
| `Total processes: {n}, Total errors: {n}` | Final stats | Info |

---

## Log Level Meanings

| Level | Shows |
|-------|-------|
| **Trace** | Everything (too verbose) |
| **Debug** | Debug info (development) |
| **Information** | Normal operation (default) |
| **Warning** | Something might be wrong |
| **Error** | Errors occurred |
| **Critical** | Serious failures |
| **None** | No logs |

---

## Filter by Component

### AudioService Only
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "AudioService": "Debug"
    }
  }
}
```

### Framework Only
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Debug"
    }
  }
}
```

---

## Extract Errors from Logs

### Docker
```bash
# Show only errors
docker logs audio-service 2>&1 | grep -i error

# Show last 100 lines with errors
docker logs audio-service 2>&1 | tail -100 | grep -i error
```

### Kubernetes
```bash
# Show only errors
kubectl logs deployment/audio-service 2>&1 | grep -i error

# Show last 100 lines with errors
kubectl logs deployment/audio-service 2>&1 | tail -100 | grep -i error
```

---

## Monitor Performance

### Success Rate
```bash
# Count processing cycles
docker logs audio-service | grep -c "Processing cycle"

# Count errors
docker logs audio-service | grep -c "Error #"
```

### Timing Information
```bash
# Show all timing information
docker logs audio-service | grep "ms"

# Average processing time
docker logs audio-service | grep "successfully" | grep "ms"
```

---

## Export Logs

### Save to File
```bash
# Docker
docker logs audio-service > logs.txt 2>&1

# Kubernetes
kubectl logs deployment/audio-service > logs.txt

# Console (redirect)
dotnet run > logs.txt 2>&1
```

### Stream to File
```bash
# Docker (continuous)
docker logs -f audio-service > logs.txt 2>&1 &

# Kubernetes (continuous)
kubectl logs -f deployment/audio-service > logs.txt &
```

---

See [`LOGGING_GUIDE.md`](LOGGING_GUIDE.md) for detailed logging documentation.
