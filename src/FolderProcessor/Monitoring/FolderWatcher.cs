using FolderProcessor.Monitoring.Watchers;

namespace FolderProcessor.Monitoring;

public class FolderWatcher
{
    private readonly IEnumerable<IWatcher> _watchers;

    public FolderWatcher(IEnumerable<IWatcher> watchers)
    {
        _watchers = watchers;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startup = _watchers
            .Select(w => w.StartAsync(cancellationToken));
        
        await Task.WhenAll(startup);
    }

    public async Task StopAsync()
    {
        var shutdown = _watchers
            .Select(w => w.StopAsync());

        await Task.WhenAll(shutdown);
    }
}