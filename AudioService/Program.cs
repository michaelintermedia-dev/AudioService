using AudioService.Services;
using Microsoft.Extensions.DependencyInjection; // Add this using directive

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient<ITaskProcessor, TaskProcessor>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });
        services.AddHostedService<AudioProcessingService>();
    });

var host = builder.Build();

// Log application startup
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("===== AudioService Starting =====");
logger.LogInformation("Environment: {environment}", host.Services.GetRequiredService<IHostEnvironment>().EnvironmentName);
logger.LogInformation("Timestamp: {timestamp:O}", DateTime.UtcNow);

await host.RunAsync();

logger.LogInformation("===== AudioService Stopped =====");
