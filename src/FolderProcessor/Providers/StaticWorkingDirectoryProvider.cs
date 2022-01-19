using System.IO.Abstractions;
using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Providers;

public class StaticWorkingDirectoryProvider : IWorkingDirectoryProvider
{
    private readonly string _folder;
    private readonly IFileSystem _fileSystem;

    public StaticWorkingDirectoryProvider(
        string folder, 
        IFileSystem fileSystem)
    {
        _folder = folder;
        _fileSystem = fileSystem;
    }

    public Task<string> GetDirectory(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_folder);
    }

    public Task<string> GetFileDestinationPath(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fileName = _fileSystem.Path.GetFileName(filePath);
        var path = _fileSystem.Path.Combine(_folder, fileName);
        if (!_fileSystem.File.Exists(path)) 
            return Task.FromResult(path);
        
        var extension = _fileSystem.Path.GetExtension(path);
        var newName = $"{Guid.NewGuid()}{extension}";
        path = _fileSystem.Path.Combine(_folder, newName);

        return Task.FromResult(path);
    }
}