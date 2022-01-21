using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Abstractions.Files;

/// <summary>
/// Moves files using a given <see cref="IDirectoryProvider"/>
/// </summary>
public interface IFileMover
{
    /// <summary>
    /// Moves a file to the location provided by the <see cref="IDirectoryProvider"/>
    /// </summary>
    /// <param name="file">The file to move</param>
    /// <param name="directoryProvider">The provider that gives the file location</param>
    /// <param name="cancellationToken">A token to cancel the task early</param>
    /// <returns>The new file path</returns>
    Task<string> MoveFileAsync(
        IFileRecord file,
        IDirectoryProvider directoryProvider,
        CancellationToken cancellationToken = default);
}