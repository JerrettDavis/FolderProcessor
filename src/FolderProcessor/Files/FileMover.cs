using System.IO.Abstractions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Providers;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Files;

public class FileMover : IFileMover
{
    private readonly ILogger<FileMover> _logger;
    private readonly IFileSystem _fileSystem;

    public FileMover(
        ILogger<FileMover> logger,
        IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public async Task<string> MoveFileAsync(
        IFileRecord file,
        IDirectoryProvider directoryProvider,
        CancellationToken cancellationToken = default)
    {
        var destination = await directoryProvider
            .GetFileDestinationPathAsync(file.Path, cancellationToken);

        if (string.Equals(file.Path, destination, StringComparison.InvariantCultureIgnoreCase))
            return destination;

        try
        {
            var directory = _fileSystem.Path.GetDirectoryName(destination);

            _logger.LogInformation("Moving file from {Source} to {Destination}", file.Path, destination);

            _fileSystem.Directory.CreateDirectory(directory);
            _fileSystem.File.Move(file.Path, destination);

            return destination;
        }
        catch (IOException ex)
        { 
            // TODO: Look into retry if file already exists (Polly) 
            _logger.LogError(
                "An Exception '{Exception}' was encountered while trying to move {File} to {Destination}",
                ex, file, destination);
            throw;
        }
    }
}