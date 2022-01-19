using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Abstractions.Files;

public interface IFileMover
{
    Task<string> MoveFileAsync(
        IFileRecord file,
        IDirectoryProvider directoryProvider,
        CancellationToken cancellationToken = default);
}