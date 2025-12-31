using Microsoft.Extensions.Logging;

namespace AudioService;

public class TaskProcessor : ITaskProcessor
{
    private readonly ILogger<TaskProcessor> _logger;

    public TaskProcessor(ILogger<TaskProcessor> logger)
    {
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing audio tasks...");
        
        try
        {
            // Add your audio processing logic here
            await PerformAudioProcessing(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Audio processing was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during audio processing");
            throw;
        }
    }

    private async Task PerformAudioProcessing(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting audio processing at {time}", DateTime.UtcNow);
        
        // Simulate audio processing work
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        
        _logger.LogInformation("Audio processing completed at {time}", DateTime.UtcNow);
    }
}
