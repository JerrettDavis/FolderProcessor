using System;
using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores
{
    /// <summary>
    /// A stored for errored file records
    /// </summary>
    public interface IErroredFileStore : IStore<Guid, IFileRecord>
    {
    
    }    
}

