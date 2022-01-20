using System.IO.Abstractions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using JetBrains.Annotations;

namespace FolderProcessor.Host.Processors;

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