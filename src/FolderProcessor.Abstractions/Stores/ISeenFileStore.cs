using System;
using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores
{
    /// <summary>
    /// A store of all files seen in a directory
    /// </summary>
    public interface ISeenFileStore : IStore<Guid, IFileRecord>
    {
        IFileRecord AddFileRecord(string filePath);
        bool ContainsPath(string filePath);
        Task<bool> ContainsPathAsync(string filePath, CancellationToken cancellationToken = default);
    }    
}

