using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

public class WorkingFileStore : Store<Guid, IFileRecord>, IWorkingFileStore
{
    
}