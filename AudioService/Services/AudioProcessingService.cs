using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioService.Services;

public class AudioProcessingService : BackgroundService
{
    private readonly ILogger<AudioProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private int _processCount = 0;
    private int _errorCount = 0;

    public AudioProcessingService(
        ILogger<AudioProcessingService> logger, 
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("========== Audio Processing Service Starting ==========");
        _logger.LogInformation("Process started at {timestamp:O}", DateTime.UtcNow);
        
        var processingInterval = _configuration.GetValue<int>("AudioService:ProcessingIntervalMinutes", 5);
        _logger.LogInformation("Processing interval configured: {intervalMinutes} minutes", processingInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Processing cycle #{processNumber} starting", _processCount + 1);
                
                // Create a scope for each iteration to properly dispose resources
                using (var scope = _serviceProvider.CreateScope())
                {
                    var taskProcessor = scope.ServiceProvider.GetRequiredService<ITaskProcessor>();
                    await taskProcessor.ProcessAsync(stoppingToken);
                }
                
                _processCount++;
                _logger.LogDebug("Processing cycle #{processNumber} completed successfully", _processCount);

                _logger.LogInformation("Next processing in {intervalMinutes} minutes", processingInterval);
                await Task.Delay(TimeSpan.FromMinutes(processingInterval), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Audio Processing Service cancellation requested");
                break;
            }
            catch (Exception ex)
            {
                _errorCount++;
                _logger.LogError(ex, "Error in Audio Processing Service (Error #{errorCount})", _errorCount);
                
                var retryDelaySeconds = _configuration.GetValue<int>("AudioService:RetryDelaySeconds", 30);
                _logger.LogWarning("Retrying after {retryDelaySeconds} seconds...", retryDelaySeconds);
                
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Retry delay cancelled");
                    break;
                }
            }
        }

        _logger.LogInformation("========== Audio Processing Service Stopped ==========");
        _logger.LogInformation("Total processes: {processCount}, Total errors: {errorCount}", _processCount, _errorCount);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Audio Processing Service graceful shutdown initiated");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Audio Processing Service graceful shutdown completed");
    }
}
