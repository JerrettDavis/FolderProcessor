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
    private readonly ConcurrentQueue<FileRecord> _queue;

    private Action<object, FileSystemEventArgs>? _tearDown;

    public FileSystemStreamHandler(
        ISeenFileStore seenFileStore, 
        ILogger<FileSystemStreamHandler> logger, 
        IPublisher publisher)
    {
        _seenFileStore = seenFileStore;
        _logger = logger;
        _publisher = publisher;
        _queue = new ConcurrentQueue<FileRecord>();
    }

    public async IAsyncEnumerable<FileRecord> Handle(
        FileSystemStream request, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var watcher = SetupWatcher(request.Folder, cancellationToken);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var record))
                yield return record;

            await Task.Delay(100, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
        
        TearDownWatcher(watcher);
    }
    
    private FileSystemWatcher SetupWatcher(string folder, CancellationToken cancellation)
    {
        var watcher = new FileSystemWatcher(folder);
        watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;
        watcher.EnableRaisingEvents = true;
        _tearDown = (_, args) => OnWatcherOnChanged(args, cancellation);
        watcher.Changed += _tearDown.Invoke;

        return watcher;
    }
    
    private async void OnWatcherOnChanged(FileSystemEventArgs args, CancellationToken cancellationToken)
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
        _queue.Enqueue(record);
        await _publisher.Publish(new FileSeenNotification { FileInfo = record }, cancellationToken);
    }

    private void TearDownWatcher(FileSystemWatcher watcher)
    {
        if (_tearDown != null)
            watcher.Changed -= _tearDown.Invoke;
    }
}