using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

public class ErroredFileStore : Store<Guid, IFileRecord>, IErroredFileStore
{
    
}