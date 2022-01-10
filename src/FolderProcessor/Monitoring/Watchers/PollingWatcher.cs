using System.Collections.Concurrent;
using FolderProcessor.Models;
using FolderProcessor.Monitoring.Notifications;
using FolderProcessor.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Watchers;

public class PollingWatcher : IDisposable, IWatcher
{
    private readonly string _folderPath;
    private readonly TimeSpan _pollInterval;
    private readonly IPublisher _publisher;

    private const int FileWaitTimeout = 250;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly ILogger<PollingWatcher> _logger;
    private readonly ISeenFileStore _seenFileStore;

    public PollingWatcher(
        string folderPath,
        TimeSpan pollInterval, 
        ISeenFileStore seenFileStore, 
        IPublisher publisher, 
        ILogger<PollingWatcher> logger)
    {
        _folderPath = folderPath;
        _pollInterval = pollInterval;
        _seenFileStore = seenFileStore;
        _publisher = publisher;
        _logger = logger;
        

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var files = Directory.EnumerateFiles(_folderPath)
                .Where(f => !_seenFileStore.Contains(f));

            await Parallel.ForEachAsync(files, cancellationToken, async (f,t) =>
            {
                var info = new FileRecord(f);
                
                _seenFileStore.Add(f);
                await _publisher.Publish(new FileSeenNotification { FileInfo = info }, t);

            });

            _logger.LogInformation("PollingWatcher watching {Directory} at: {Time}", _folderPath, DateTimeOffset.Now);
            await Task.Delay(_pollInterval, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
    }

    public Task StopAsync()
    {
        _cancellationTokenSource.Cancel();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}