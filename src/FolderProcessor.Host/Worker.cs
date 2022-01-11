using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Streams;

namespace FolderProcessor.Host;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var services = _serviceProvider.CreateAsyncScope();
        var watcher = services.ServiceProvider.GetService<StreamedFolderWatcher>()!;

        await watcher.StartAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }

        await watcher.StopAsync();
    }
}