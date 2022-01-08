using FolderProcessor.Stores;

namespace FolderProcessor.Watchers;

public class PollingWatcher : IDisposable
{
    private readonly string _folderPath;
    private readonly TimeSpan _pollInterval;
    private readonly ISeenFileStore _seenFileStore;

    private readonly CancellationTokenSource _cancellationTokenSource;

    public PollingWatcher(
        string folderPath,
        TimeSpan pollInterval, 
        ISeenFileStore seenFileStore)
    {
        _folderPath = folderPath;
        _pollInterval = pollInterval;
        _seenFileStore = seenFileStore;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            
            
            await Task.Delay(_pollInterval, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}