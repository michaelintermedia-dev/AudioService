using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioService;

public class AudioProcessingService : BackgroundService
{
    private readonly ILogger<AudioProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AudioProcessingService(ILogger<AudioProcessingService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audio Processing Service starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a scope for each iteration to properly dispose resources
                using (var scope = _serviceProvider.CreateScope())
                {
                    var taskProcessor = scope.ServiceProvider.GetRequiredService<ITaskProcessor>();
                    await taskProcessor.ProcessAsync(stoppingToken);
                }

                // Wait before next execution (e.g., 5 minutes)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Audio Processing Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Audio Processing Service");
                // Wait before retry
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Audio Processing Service stopping gracefully");
        await base.StopAsync(cancellationToken);
    }
}
