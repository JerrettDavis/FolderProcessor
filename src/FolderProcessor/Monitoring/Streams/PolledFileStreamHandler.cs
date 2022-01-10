using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<FileRecord> Handle(
        PolledFileStream request, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queue = new ConcurrentQueue<FileRecord>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var files = Directory.EnumerateFiles(request.Folder)
                .Where(f => !_seenFileStore.Contains(f));

            await Parallel.ForEachAsync(files, CancellationToken.None, async (f,t) =>
            {
                var info = new FileRecord(f);
                
                _seenFileStore.Add(f);
                await _publisher.Publish(new FileSeenNotification { FileInfo = info }, t);
                queue.Enqueue(info);
            });
            
            // TODO: Try mixing the above parallel task with the serving task... Might be chaos...

            while (!queue.IsEmpty)
            {
                if (queue.TryDequeue(out var result))
                    yield return result;
            }

            _logger.LogInformation("PolledFileStreamHandler watching {Directory} at: {Time}", request.Folder, DateTimeOffset.Now);
            
            await Task.Delay(request.Interval, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
    }
}