using System.IO.Abstractions;
using System.Threading.Channels;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Stores;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams;

/// <summary>
/// Instructs the application to setup a new <see cref="PolledFileStreamHandler"/>
/// matching the passed parameters.
/// </summary>
[PublicAPI]
public class PolledFileStream : IFileStream
{
    /// <inheritdoc />
    public string Folder { get; set; } = null!;
    
    /// <summary>
    /// The frequency in which to poll the folder for new files.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>
/// Continually polls a given folder for new files.
/// </summary>
[UsedImplicitly]
public class PolledFileStreamHandler : 
    IStreamRequestHandler<PolledFileStream, IFileRecord>
{
    private readonly ISeenFileStore _seenFileStore;
    private readonly IPublisher _publisher;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<PolledFileStreamHandler> _logger;
    
    public PolledFileStreamHandler(
        ISeenFileStore seenFileStore, 
        IPublisher publisher, 
        ILogger<PolledFileStreamHandler> logger, 
        IFileSystem fileSystem)
    {
        _seenFileStore = seenFileStore;
        _publisher = publisher;
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public IAsyncEnumerable<IFileRecord> Handle(
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
            CancellationToken.None); // Poller is self-killing
        cancellationToken.Register(() => channel.Writer.Complete());
        
        return channel.Reader.ReadAllAsync(CancellationToken.None);
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
                await Task.WhenAll(
                    _fileSystem.Directory.EnumerateFiles(folder)
                        .Where(f => !_seenFileStore.Contains(f))
                        .AsParallel()
                        .Select(async f =>
                            await FileSeen(f, channel, cancellationToken))
                        .ToList());

                _logger.LogInformation("Polled {Directory} at: {Time}",
                    folder, DateTimeOffset.Now);

                await Task.Delay(interval, cancellationToken)
                    .ContinueWith(_ => { }, CancellationToken.None);
            }
            catch (DirectoryNotFoundException)
            {
                await _publisher.Publish(
                    new DirectoryNotFoundNotification {Path = folder},
                        cancellationToken);
                _logger.LogError(
                    "Folder {Folder} cannot be found. Shutting down poller.",
                    folder);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "An error occurred while running file polling mechanism." +
                    "Polling will continue, but files may be skipped...");

                await Task.Delay(interval, CancellationToken.None);
            }
        }
    }
    
    private async Task FileSeen(
        string path, 
        Channel<FileRecord, FileRecord> channel,
        CancellationToken cancellationToken)
    {
        var info = new FileRecord(path);
                        
        _seenFileStore.Add(path);
        await _publisher.Publish(
            new FileSeenNotification {FileInfo = info}, 
            cancellationToken);

        await channel.Writer.WriteAsync(info, cancellationToken);
    }
}