using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

public class CompletedFileStore : Store<Guid, IFileRecord>, ICompletedFileStore
{
    
}