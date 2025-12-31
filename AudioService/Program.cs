using AudioService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<ITaskProcessor, TaskProcessor>();
        services.AddHostedService<AudioProcessingService>();
    });

var host = builder.Build();
await host.RunAsync();
