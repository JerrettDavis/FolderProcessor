using System;
using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores
{
    /// <summary>
    /// A store for completed file records
    /// </summary>
    public interface ICompletedFileStore : IStore<Guid, IFileRecord>
    {
    
    }    
}