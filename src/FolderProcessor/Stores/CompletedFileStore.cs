using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

/// <summary>
/// A store containing all completed file records.
/// </summary>
public class CompletedFileStore : Store<Guid, IFileRecord>, ICompletedFileStore
{
    
}