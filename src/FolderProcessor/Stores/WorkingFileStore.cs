using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;

namespace FolderProcessor.Stores;

/// <summary>
/// A store containing all files currently being worked on.
/// </summary>
public class WorkingFileStore : Store<Guid, IFileRecord>, IWorkingFileStore
{
    
}