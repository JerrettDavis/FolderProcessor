using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Filters;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Filters;

/// <summary>
/// A filter that attempts to open and read a file's contents. If it cannot be
/// opened, it's discarded.
/// </summary>
public class FileExistsFilter : IFileFilter
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<FileExistsFilter> _logger;

    public FileExistsFilter(
        IFileSystem fileSystem,
        ILogger<FileExistsFilter> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public Task<bool> IsValid(
        string input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(_fileSystem.File.Exists(input));
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, 
                "Encountered an error while trying to check if {Path} exists...", 
                input);
            
            return Task.FromResult(false);
        }
    }
}