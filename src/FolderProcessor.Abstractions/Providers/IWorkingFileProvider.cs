using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Providers;

public interface IWorkingFileProvider
{
    Task AddAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
    Task<IFileRecord> GetAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
    Stream GetStream(IFileRecord fileRecord, CancellationToken cancellationToken);
    Task RemoveAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
}