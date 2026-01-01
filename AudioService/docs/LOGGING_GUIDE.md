# Logging Guide - AudioService

## Overview

AudioService includes comprehensive logging throughout the application using .NET's built-in logging infrastructure. All logs are output to the console, making them easily accessible in Docker, Kubernetes, and local development environments.

---

## Logging Architecture

### Providers

The application uses the following logging providers:

- **Console Provider** - Outputs logs to stdout/stderr
- **Debug Provider** - Outputs logs to debug output (IDE)

### Log Levels

Log levels are configured per namespace:

| Namespace | Level | Purpose |
|-----------|-------|---------|
| `Microsoft.*` | Warning | Framework warnings only |
| `Microsoft.Hosting.Lifetime` | Information | Hosting lifecycle events |
| `AudioService.*` | Information | Application business logic |

---

## Application Logging Points

### Program.cs

**Startup Logging:**
```
===== AudioService Starting =====
Environment: Production
Timestamp: 2024-01-15T10:30:45.1234567Z
```

**Shutdown Logging:**
```
===== AudioService Stopped =====
```

### AudioProcessingService

Tracks the service lifecycle:

```
========== Audio Processing Service Starting ==========
Process started at 2024-01-15T10:30:45.1234567Z
Processing interval configured: 5 minutes

[Processing cycle...]

========== Audio Processing Service Stopped ==========
Total processes: 42, Total errors: 1
```

**Key Logging Events:**
- Service startup with configuration
- Processing cycle start/completion
- Errors with retry delays
- Service shutdown with statistics

### TaskProcessor

Measures and logs individual processing tasks:

```
----> Processing audio tasks started
  >> Audio processing routine starting at 2024-01-15T10:30:45.1234567Z
  >> Simulating audio processing work...
  >> Audio processing routine completed at 2024-01-15T10:30:46.1234567Z
----> Processing audio tasks completed successfully in 1001ms
```

**Key Logging Events:**
- Task start and completion
- Execution timing (milliseconds)
- Detailed routine steps
- Error handling with context

---

## Log Output Examples

### Console Output (Development)

```
info: AudioService.Program[0]
      ===== AudioService Starting =====
info: AudioService.Program[0]
      Environment: Development
info: AudioService.Program[0]
      Timestamp: 2024-01-15T10:30:45.1234567Z
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: AudioService.Services.AudioProcessingService[0]
      ========== Audio Processing Service Starting ==========
info: AudioService.Services.AudioProcessingService[0]
      Process started at 2024-01-15T10:30:45.1234567Z
info: AudioService.Services.AudioProcessingService[0]
      Processing interval configured: 1 minutes
dbug: AudioService.Services.AudioProcessingService[0]
      Processing cycle #1 starting
info: AudioService.Services.TaskProcessor[0]
      ----> Processing audio tasks started
info: AudioService.Services.TaskProcessor[0]
        >> Audio processing routine starting at 2024-01-15T10:30:45.1234567Z
dbug: AudioService.Services.TaskProcessor[0]
        >> Simulating audio processing work...
info: AudioService.Services.TaskProcessor[0]
        >> Audio processing routine completed at 2024-01-15T10:30:46.1234567Z
info: AudioService.Services.TaskProcessor[0]
      ----> Processing audio tasks completed successfully in 1001ms
dbug: AudioService.Services.AudioProcessingService[0]
      Processing cycle #1 completed successfully
info: AudioService.Services.AudioProcessingService[0]
      Next processing in 1 minutes
```

### Docker Output

```
docker logs -f audio-service

info: AudioService.Program[0]
      ===== AudioService Starting =====
...
```

### Kubernetes Output

```
kubectl logs -f deployment/audio-service

info: AudioService.Program[0]
      ===== AudioService Starting =====
...
```

---

## Configuration

### Logging Levels

Configure log levels in `appsettings.json`:

**Development (Verbose)**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Debug"
    }
  }
}
```

**Production (Minimal)**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Error",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### Environment-Based Configuration

The application uses environment-specific settings:

```bash
# Development - Verbose logging
DOTNET_ENVIRONMENT=Development dotnet run

# Production - Minimal logging
DOTNET_ENVIRONMENT=Production dotnet run
```

---

## Structured Logging

The application uses structured logging for better filtering and analysis:

```csharp
_logger.LogInformation("Processing interval configured: {intervalMinutes} minutes", 5);
_logger.LogError(ex, "Error in Audio Processing Service (Error #{errorCount})", errorCount);
_logger.LogWarning("Retrying after {retryDelaySeconds} seconds...", 30);
```

### Structured Data Examples

```
interval: 5
errorCount: 1
retryDelaySeconds: 30
milliseconds: 1001
processNumber: 42
timestamp: 2024-01-15T10:30:45.1234567Z
```

---

## Monitoring Logs

### Docker Compose

```bash
# View all logs
docker-compose logs audio-service

# Follow logs in real-time
docker-compose logs -f audio-service

# View last 100 lines
docker-compose logs --tail=100 audio-service
```

### Docker

```bash
# View logs
docker logs audio-service

# Follow logs
docker logs -f audio-service

# View with timestamps
docker logs --timestamps audio-service
```

### Kubernetes

```bash
# View pod logs
kubectl logs deployment/audio-service

# Follow logs
kubectl logs -f deployment/audio-service

# View specific pod
kubectl logs pod-name

# View previous pod (if crashed)
kubectl logs pod-name --previous
```

### Console

Logs appear directly in the terminal console output.

---

## Log Levels Guide

| Level | Usage | Example |
|-------|-------|---------|
| **Trace** | Most verbose, detailed diagnostics | Method entry/exit |
| **Debug** | Debug information | Cycle start/end |
| **Information** | General information | Service start, configuration |
| **Warning** | Warning messages | Retry delays, config issues |
| **Error** | Error messages | Processing failures |
| **Critical** | Critical errors | Service crashes |
| **None** | No logging | - |

---

## Best Practices

### When Adding Logging

? **DO:**
- Log significant events (startup, shutdown, errors)
- Include timing information for performance
- Use structured logging with properties
- Log configuration values at startup
- Include error context in exception logs

? **DON'T:**
- Log sensitive data (passwords, tokens, PII)
- Log in tight loops (performance impact)
- Log at Information level for routine operations
- Use concatenation instead of structured logging

### Example: Good Logging

```csharp
// ? Good - Structured, contextual, includes timing
var stopwatch = Stopwatch.StartNew();
_logger.LogInformation("Processing task {taskId} started", taskId);

try 
{
    await ProcessTask(cancellationToken);
    stopwatch.Stop();
    _logger.LogInformation("Processing task {taskId} completed in {ms}ms", taskId, stopwatch.ElapsedMilliseconds);
}
catch (Exception ex)
{
    stopwatch.Stop();
    _logger.LogError(ex, "Processing task {taskId} failed after {ms}ms", taskId, stopwatch.ElapsedMilliseconds);
    throw;
}
```

### Example: Poor Logging

```csharp
// ? Poor - String concatenation, no structure, no context
_logger.LogInformation("Processing task " + taskId);
// ... 
_logger.LogError("Error: " + ex.Message);
```

---

## Analyzing Logs

### Common Log Patterns

**Successful Processing Cycle:**
```
info: AudioProcessingService Processing cycle #1 starting
info: TaskProcessor Processing audio tasks started
info: TaskProcessor Audio processing routine starting
info: TaskProcessor Audio processing routine completed
info: TaskProcessor Processing audio tasks completed successfully in 1001ms
info: AudioProcessingService Processing cycle #1 completed successfully
```

**Error and Retry:**
```
error: AudioProcessingService Error in Audio Processing Service (Error #1)
warn: AudioProcessingService Retrying after 30 seconds...
```

**Shutdown:**
```
info: AudioProcessingService Audio Processing Service graceful shutdown initiated
info: AudioProcessingService Audio Processing Service graceful shutdown completed
info: Program ===== AudioService Stopped =====
```

---

## Troubleshooting

### No Logs Appearing

**Docker:**
```bash
# Ensure you're following container logs
docker logs -f audio-service

# Check container is running
docker ps | grep audio-service
```

**Kubernetes:**
```bash
# Check pod is running
kubectl get pods

# View logs
kubectl logs pod-name

# Check pod events
kubectl describe pod pod-name
```

**Console:**
- Logs should appear in terminal
- Check log level configuration
- Ensure application is running in foreground

### Too Many Logs

Adjust log levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "AudioService": "Information"
    }
  }
}
```

Or set environment variable:

```bash
# Linux
export Logging__LogLevel__Default=Warning
dotnet run

# Windows PowerShell
$env:Logging__LogLevel__Default='Warning'
dotnet run
```

### Missing Logs

Check if log level is too high:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Error"  // Only errors and critical
    }
  }
}
```

---

## Performance Impact

Logging has minimal performance impact:

- **Console Provider** - Fast, suitable for production
- **Debug Provider** - Only active when debugging
- **Structured Logging** - Negligible overhead

---

## Next Steps

1. Deploy application
2. Monitor logs in production environment
3. Adjust log levels as needed
4. Use logs for troubleshooting and monitoring
5. Export logs to log aggregation service (optional)

See [`QUICK_REFERENCE.md`](QUICK_REFERENCE.md) for logging commands.
