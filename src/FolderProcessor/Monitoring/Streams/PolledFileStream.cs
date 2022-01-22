using System.IO.Abstractions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Common.Utilities;
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
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<PolledFileStream> _logger;

    public PolledFileStreamHandler(
        ISeenFileStore seenFileStore,
        IFileSystem fileSystem, 
        ILogger<PolledFileStream> logger)
    {
        _seenFileStore = seenFileStore;
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public IAsyncEnumerable<IFileRecord> Handle(
        PolledFileStream request, 
        CancellationToken cancellationToken)
    {
        return AppTaskExt.Timer(request.Interval, cancellationToken)
            .Do(_ => _logger.LogInformation("Poller for {Directory} running", request.Folder))
            .Select(_ => _fileSystem.Directory
                .EnumerateFiles(request.Folder)
                .AsParallel()
                .Where(f => !_seenFileStore.ContainsPath(f))
                .Select(_seenFileStore.AddFileRecord))
            .SelectMany(f => f.ToAsyncEnumerable());
    }
}