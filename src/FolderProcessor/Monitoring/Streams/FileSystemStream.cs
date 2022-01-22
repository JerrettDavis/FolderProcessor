using System.IO.Abstractions;
using System.Threading.Channels;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Abstractions.Stores;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams;

/// <summary>
/// Instructs the application to setup a new <see cref="FileSystemStreamHandler"/>
/// matching the passed parameters.
/// </summary>
public class FileSystemStream : IFileStream
{
    /// <inheritdoc />
    public string Folder { get; set; } = null!;
}

/// <summary>
/// Uses the <see cref="FileSystemWatcher"/> to continuously monitor a folder
/// for new files.
/// </summary>
[UsedImplicitly]
public class FileSystemStreamHandler : 
    IStreamRequestHandler<FileSystemStream, IFileRecord>
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly ILogger<FileSystemStreamHandler> _logger;
    private readonly IFileSystem _fileSystem;

    public FileSystemStreamHandler(
        ISeenFileStore seenFileStore, 
        ILogger<FileSystemStreamHandler> logger, 
        IFileSystem fileSystem)
    {
        _seenFileStore = seenFileStore;
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public IAsyncEnumerable<IFileRecord> Handle(
        FileSystemStream request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started monitoring {Folder}", request.Folder);
        var watcher = SetupWatcher(request.Folder);

        return EventToAsyncEnumerable(watcher, cancellationToken)
            .WhereAwaitWithCancellation(async (f,t) => 
                !await _seenFileStore
                    .ContainsPathAsync(f, t))
            .Select(_seenFileStore.AddFileRecord)
            .WhereAwaitWithCancellation(async (f,t) => 
                !await IsDirectory(f.Id, f.Path, t));
    }
    
    private IFileSystemWatcher SetupWatcher(string folder)
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

        return watcher;
    }

    private static IAsyncEnumerable<string> EventToAsyncEnumerable(
        IFileSystemWatcher watcher,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();
        
        cancellationToken.Register(() => channel.Writer.Complete());
        watcher.Created += async (_, args) => await 
            channel.Writer.WriteAsync(args.FullPath, cancellationToken);

        return channel.Reader.ReadAllAsync(CancellationToken.None);
    }

    private async Task<bool> IsDirectory(Guid id, string path, CancellationToken cancellationToken)
    {
        try
        {
            return (_fileSystem.File.GetAttributes(path) & 
                    FileAttributes.Directory) != 0;
            
        } catch (Exception ex) when 
            (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            _logger.LogWarning(
                "File {File} was not found. During a routine pre-check. " +
                "Will not be broadcast", path);
            
            await _seenFileStore
                .RemoveAsync(id, cancellationToken)
                .ConfigureAwait(false);

            // Skip it like a directory
            return true;
        }
    }
}