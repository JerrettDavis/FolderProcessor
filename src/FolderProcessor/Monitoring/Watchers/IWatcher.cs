namespace FolderProcessor.Monitoring.Watchers;

public interface IWatcher
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();
}