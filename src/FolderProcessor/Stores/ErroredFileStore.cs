using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

/// <summary>
/// A store containing all errored file records
/// </summary>
public class ErroredFileStore : Store<Guid, IFileRecord>, IErroredFileStore
{
    
}