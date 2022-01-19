using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores;

public interface IWorkingFileStore : IStore<Guid, IFileRecord>
{
}