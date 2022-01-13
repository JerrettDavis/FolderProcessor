using System.IO.Abstractions;
using System.Threading.Channels;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams;

/// <summary>
/// Uses the <see cref="FileSystemWatcher"/> to continuously monitor a folder
/// for new files.
/// </summary>
public class FileSystemStreamHandler : 
    IStreamRequestHandler<FileSystemStream, IFileRecord>
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly ILogger<FileSystemStreamHandler> _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IPublisher _publisher;

    public FileSystemStreamHandler(
        ISeenFileStore seenFileStore, 
        ILogger<FileSystemStreamHandler> logger, 
        IFileSystem fileSystem,
        IPublisher publisher)
    {
        _seenFileStore = seenFileStore;
        _logger = logger;
        _fileSystem = fileSystem;
        _publisher = publisher;
    }

    public IAsyncEnumerable<IFileRecord> Handle(
        FileSystemStream request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started monitoring {Folder}", request.Folder);
        
        var channel = Channel.CreateUnbounded<FileRecord>();
        var watcher = SetupWatcher(request.Folder, channel, cancellationToken);
        
        cancellationToken.Register(() =>
        {
            watcher.Dispose();
            channel.Writer.Complete();
        });
        
        // The above watcher setup handles the shutdown gracefully for us.
        return channel.Reader.ReadAllAsync(CancellationToken.None);
    }
    
    private IFileSystemWatcher SetupWatcher(
        string folder, 
        Channel<FileRecord, FileRecord> channel,
        CancellationToken cancellationToken)
    {
        var watcher = _fileSystem.FileSystemWatcher.CreateNew(folder);
        watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
        watcher.EnableRaisingEvents = true;
        watcher.Created += (_, args) => OnWatcherOnChanged(args, channel, cancellationToken);

        return watcher;
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
            // We receive events about directories, but we don't care about them.
            if ((_fileSystem.File.GetAttributes(path) & 
                 FileAttributes.Directory) != 0) return;
            
            var record = new FileRecord(path);
        
            await channel.Writer.WriteAsync(record, cancellationToken);
            await _publisher.Publish(
                new FileSeenNotification { FileInfo = record }, 
                cancellationToken);
        }
        catch (Exception ex) when 
            (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            _logger.LogWarning(
                "File {File} was not found. During a routine pre-check. " +
                "Will not be broadcast", path);
            
            _seenFileStore.Remove(path);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Watcher shutdown while {File} was being handled", 
                args.FullPath);
        }
    }
}