using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores;

public interface IErroredFileStore : IStore<Guid, IFileRecord>
{
    
}