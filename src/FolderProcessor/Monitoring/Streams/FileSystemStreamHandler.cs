using System.Threading.Channels;
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

    public IAsyncEnumerable<FileRecord> Handle(
        FileSystemStream request, 
        CancellationToken cancellationToken)
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
        watcher.EnableRaisingEvents = true;

        var buffer = Channel.CreateUnbounded<FileRecord>();
        watcher.Created += (_, args) => OnWatcherOnChanged(args, buffer, cancellationToken);

        return buffer.Reader.ReadAllAsync(cancellationToken);
    }


    private async void OnWatcherOnChanged(
        FileSystemEventArgs args, 
        Channel<FileRecord, FileRecord> channel,
        CancellationToken cancellationToken)
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
            _logger.LogWarning("File {File} was not found. During a routine pre-check. Will not be broadcast", path);
            return;
        }
            
        var record = new FileRecord(path);
        var written = channel.Writer.TryWrite(record);
        if (!written)
            _logger.LogError("File {file} was not written to channel!", record);
        await _publisher.Publish(new FileSeenNotification { FileInfo = record }, cancellationToken);
    }
}