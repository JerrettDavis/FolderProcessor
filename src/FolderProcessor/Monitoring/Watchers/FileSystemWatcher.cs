using FolderProcessor.Models;
using FolderProcessor.Monitoring.Notifications;
using FolderProcessor.Stores;
using MediatR;
using Microsoft.Extensions.Logging;
using FSWatcher = System.IO.FileSystemWatcher;

namespace FolderProcessor.Monitoring.Watchers;

public class FileSystemWatcher : IWatcher
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly IPublisher _publisher;
    private readonly FSWatcher _watcher;
    private readonly ILogger<FileSystemWatcher> _logger;

    public FileSystemWatcher(
        string folderPath, 
        ISeenFileStore seenFileStore, 
        IPublisher publisher, 
        ILogger<FileSystemWatcher> logger)
    {
        _seenFileStore = seenFileStore;
        _publisher = publisher;
        _logger = logger;

        _watcher = new FSWatcher(folderPath);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
        
        _watcher.EnableRaisingEvents = true;
        _watcher.Changed += OnFileCreated;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Changed -= OnFileCreated;

        return Task.CompletedTask;
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs args)
    {
        var path = args.FullPath;
        
        if (_seenFileStore.Contains(path))
            return;

        try
        {
            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
                return;
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("File {File} was not found. During a routine check. Will not be broadcast", path);
            return;
        }
        
        _seenFileStore.Add(path);
        var record = new FileRecord(path);
        await _publisher.Publish(new FileSeenNotification { FileInfo = record});
    }
}