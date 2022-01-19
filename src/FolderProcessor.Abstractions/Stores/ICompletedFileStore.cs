using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores;

public interface ICompletedFileStore : IStore<Guid, IFileRecord>
{
    
}