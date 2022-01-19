using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores;

public interface IWorkingFileStore
{
    Task AddAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
    Task<IFileRecord> GetAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
    Stream GetStream(IFileRecord fileRecord, CancellationToken cancellationToken);
    Task RemoveAsync(IFileRecord fileRecord, CancellationToken cancellationToken);
}