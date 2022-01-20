using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Filters;

namespace FolderProcessor.Monitoring.Filters;

/// <summary>
/// Creates a filter that only accepts files matching the file extension,
/// including "." (e.g. ".txt"). 
/// </summary>
public class FileTypeFileFilter : IFileFilter
{
    private readonly IEnumerable<string> _extensions;
    private readonly IFileSystem _fileSystem;

    public FileTypeFileFilter(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _extensions = Array.Empty<string>();
    }
    
    public FileTypeFileFilter(
        IFileSystem fileSystem, 
        IEnumerable<string> extensions)
    {
        _extensions = extensions;
        _fileSystem = fileSystem;
    }

    /// <summary>
    /// Returns whether the input matches one of the configured file extensions.
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <param name="cancellationToken">A token to cancel the task</param>
    /// <returns>True if the file has the correct extension.</returns>
    public Task<bool> IsValid(
        string input,
        CancellationToken cancellationToken = default)
    {
        if (!_extensions.Any()) return Task.FromResult(true);
        var extension = _fileSystem.Path.GetExtension(input);

        return Task.FromResult(_extensions.Contains(extension));
    }
}