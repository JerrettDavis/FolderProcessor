using System.Threading.Channels;
using FolderProcessor.Models;
using FolderProcessor.Monitoring.Notifications;
using FolderProcessor.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams;

public class PolledFileStreamHandler : 
    IStreamRequestHandler<PolledFileStream, FileRecord>
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly IPublisher _publisher;
    private readonly ILogger<PolledFileStreamHandler> _logger;

    public PolledFileStreamHandler(
        ISeenFileStore seenFileStore, 
        IPublisher publisher, 
        ILogger<PolledFileStreamHandler> logger)
    {
        _seenFileStore = seenFileStore;
        _publisher = publisher;
        _logger = logger;
    }

    public IAsyncEnumerable<FileRecord> Handle(
        PolledFileStream request, 
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<FileRecord>();
        
        // Defer the poll to a background worker so our channel can return.
        Task.Run(async () => 
            await FolderPoller(
                request.Folder, 
                request.Interval, 
                channel, 
                cancellationToken), 
            cancellationToken);
        
        return channel.Reader.ReadAllAsync(cancellationToken);
    }

    private async Task FolderPoller(
        string folder,
        TimeSpan interval,
        Channel<FileRecord, FileRecord> channel,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var files = Directory.EnumerateFiles(folder)
                    .Where(f => !_seenFileStore.Contains(f));

                /* We want to eagerly grab and broadcast the presence of all the files
                 * we find. But since we're operating as a stream, we can't immediately 
                 * return them to the caller. To remedy this, we put all the files we
                 * find in a Channel that emits items to the caller as they're request. */
                await Parallel.ForEachAsync(files, CancellationToken.None, async (f, t) =>
                {
                    var info = new FileRecord(f);

                    _seenFileStore.Add(f);
                    await _publisher.Publish(new FileSeenNotification {FileInfo = info}, t);
                    if (!channel.Writer.TryWrite(info))
                        _logger.LogError("Unable to add {File} to Channel!", info);

                });

                _logger.LogInformation("PolledFileStreamHandler watching {Directory} at: {Time}", folder,
                    DateTimeOffset.Now);

                await Task.Delay(interval, cancellationToken)
                    .ContinueWith(_ => { }, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running file watcher poll.");
            }
        }
    }
}