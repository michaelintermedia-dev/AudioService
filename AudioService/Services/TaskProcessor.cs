using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AudioService.Services;

public class TaskProcessor : ITaskProcessor
{
    private readonly ILogger<TaskProcessor> _logger;

    public TaskProcessor(ILogger<TaskProcessor> logger)
    {
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("----> Processing audio tasks started");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Add your audio processing logic here
            await PerformAudioProcessing(cancellationToken);
            
            stopwatch.Stop();
            _logger.LogInformation("----> Processing audio tasks completed successfully in {milliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("Audio processing was cancelled after {milliseconds}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error during audio processing after {milliseconds}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task PerformAudioProcessing(CancellationToken cancellationToken)
    {
        _logger.LogInformation("  >> Audio processing routine starting at {time:O}", DateTime.UtcNow);
        
        try
        {
            // TODO: Replace with your actual audio processing logic
            _logger.LogDebug("  >> Simulating audio processing work...");
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            
            _logger.LogInformation("  >> Audio processing routine completed at {time:O}", DateTime.UtcNow);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("  >> Audio processing routine was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "  >> Exception in audio processing routine");
            throw;
        }
    }
}
