using System.Collections.Concurrent;

namespace FolderProcessor.Stores;

/// <summary>
/// Maintains a thread-safe <see cref="ConcurrentDictionary{TKey,TValue}"/> to
/// maintain a collection of all seen files.
/// </summary>
public class SeenFileStore : ISeenFileStore
{
    private readonly ConcurrentDictionary<string, FileInfo> _seen = new();
    
    /// <inheritdoc />
    public void Add(string fileName)
    {
        _seen.AddOrUpdate(fileName, v => new FileInfo(v), (_, info) => info);
    }

    /// <inheritdoc />
    public bool Contains(string fileName)
    {
        return _seen.ContainsKey(fileName);
    }

    /// <inheritdoc />
    public void Remove(string fileName)
    {
        _seen.Remove(fileName, out _);
    }
}