using System.Collections.Concurrent;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Common.Exceptions;
using FolderProcessor.Models.Files;

namespace FolderProcessor.Stores;

/// <summary>
/// Maintains a thread-safe <see cref="ConcurrentDictionary{TKey,TValue}"/> to
/// maintain a collection of all seen files.
/// </summary>
public class SeenFileStore : ISeenFileStore
{
    private readonly ConcurrentDictionary<Guid, IFileRecord> _seen = new();

    public void Add(Guid key, IFileRecord item)
    {
        if (!_seen.TryAdd(key, item))
            throw new AddToStoreException(key.ToString(), item);
    }

    public Task AddAsync(
        Guid key, 
        IFileRecord item, 
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Add(key, item), cancellationToken);
    }

    public bool Contains(Guid key)
    {
        return _seen.ContainsKey(key);
    }

    public Task<bool> ContainsAsync(
        Guid key, 
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Contains(key), cancellationToken);
    }

    public IFileRecord Get(Guid key)
    {
        if (!_seen.TryGetValue(key, out var item))
            throw new GetFromStoreException(key);

        return item;
    }

    public Task<IFileRecord> GetAsync(
        Guid key, 
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Get(key), cancellationToken);
    }

    public void Remove(Guid key)
    {
        if (!_seen.TryRemove(key, out _))
            throw new RemoveFromStoreException(key.ToString());
    }

    public Task RemoveAsync(
        Guid key, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IFileRecord AddFileRecord(string filePath)
    {
        var record = new FileRecord(filePath);
        
        Add(record.Id, record);
        return record;
    }

    public bool ContainsPath(string filePath)
    {
        return _seen.Values.Any(p => p.Path.Equals(filePath));
    }

    public Task<bool> ContainsPathAsync(
        string filePath, 
        CancellationToken cancellationToken)
    {
        return Task.Run(() => ContainsPath(filePath), cancellationToken);
    }
}