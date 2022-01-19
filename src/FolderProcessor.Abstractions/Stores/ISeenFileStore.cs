using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Stores;

/// <summary>
/// Maintains a thread-sfe collection of all files seen by various watchers
/// </summary>
public interface ISeenFileStore : IStore<Guid, IFileRecord>
{
    IFileRecord AddFileRecord(string filePath);
    bool ContainsPath(string filePath);
    Task<bool> ContainsPathAsync(string filePath, CancellationToken cancellationToken = default);
}