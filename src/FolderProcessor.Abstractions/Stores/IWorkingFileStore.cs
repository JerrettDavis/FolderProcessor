using System;
using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores
{
    /// <summary>
    /// A store of all files currently being worked on
    /// </summary>
    public interface IWorkingFileStore : IStore<Guid, IFileRecord>
    {
    }    
}