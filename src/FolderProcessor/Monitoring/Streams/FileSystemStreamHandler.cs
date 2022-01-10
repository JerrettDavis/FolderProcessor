using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using FolderProcessor.Models;
using FolderProcessor.Monitoring.Notifications;
using FolderProcessor.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams;

public class FileSystemStreamHandler : 
    IStreamRequestHandler<FileSystemStream, FileRecord>
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly ILogger<FileSystemStreamHandler> _logger;
    private readonly IPublisher _publisher;

    public FileSystemStreamHandler(
        ISeenFileStore seenFileStore, 
        ILogger<FileSystemStreamHandler> logger, 
        IPublisher publisher)
    {
        _seenFileStore = seenFileStore;
        _logger = logger;
        _publisher = publisher;
    }

    public async IAsyncEnumerable<FileRecord> Handle(
        FileSystemStream request, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var watcher = new FileSystemWatcher(request.Folder);
        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
        
        var queue = new ConcurrentQueue<FileRecord>();
        watcher.EnableRaisingEvents = true;

        async void OnWatcherOnChanged(object _, FileSystemEventArgs args)
        {
            var path = args.FullPath;

            if (_seenFileStore.Contains(path)) return;
            
            _seenFileStore.Add(path);

            try
            {
                if ((File.GetAttributes(path) & FileAttributes.Directory) != 0) return;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("File {File} was not found. During a routine check. Will not be broadcast", path);
                return;
            }
            
            var record = new FileRecord(path);
            queue.Enqueue(record);
            await _publisher.Publish(new FileSeenNotification { FileInfo = record }, cancellationToken);
        }

        watcher.Changed += OnWatcherOnChanged;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (queue.TryDequeue(out var record))
                yield return record;

            await Task.Delay(100, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
    }
}