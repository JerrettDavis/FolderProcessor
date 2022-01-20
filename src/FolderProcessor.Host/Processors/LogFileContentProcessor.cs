using System.IO.Abstractions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using JetBrains.Annotations;

namespace FolderProcessor.Host.Processors;

/// <summary>
/// A demonstration <see cref="IProcessor"/> that simply logs the contents of
/// every file it encounters.
/// </summary>
[UsedImplicitly]
public class LogFileContentProcessor : IProcessor
{
    private readonly ILogger<LogFileContentProcessor> _logger;
    private readonly IFileSystem _fileSystem;

    public LogFileContentProcessor(
        ILogger<LogFileContentProcessor> logger, 
        IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public async Task Process(
        IFileRecord fileRecord, 
        CancellationToken cancellationToken = default)
    {
        // This is DANGEROUS. Processors are ran concurrently, and this could cause
        // the file to get locked if multiple processors are attempting to access
        // it simultaneously.
        var content = await _fileSystem.File
            .ReadAllTextAsync(fileRecord.Path, cancellationToken);
        
        _logger.LogInformation("File {File}. Content '{Content}'", fileRecord, content);
    }
    
    public Task<bool> AppliesAsync(
        IFileRecord fileRecord, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}