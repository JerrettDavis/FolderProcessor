using FolderProcessor.Monitoring;

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
        
        _logger.LogInformation("Firing up the folder watcher..");
        await watcher.StartAsync(stoppingToken);
    }
}