using System.Collections.Concurrent;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Common.Exceptions;

namespace FolderProcessor.Stores;

/// <summary>
/// Provides a base thread-safe store for storing generic objects.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TType">The type of the object being stored.</typeparam>
public abstract class Store<TKey, TType> : IStore<TKey, TType>
    where TKey : notnull
    where TType : class
{
    protected readonly ConcurrentDictionary<TKey, TType> Dictionary = new();

    public virtual void Add(TKey key, TType item)
    {
        if (!Dictionary.TryAdd(key, item))
            throw new AddToStoreException(key, item);
    }

    public virtual Task AddAsync(
        TKey key,
        TType item,
        CancellationToken cancellationToken = default)
    {
        Add(key, item);
        return Task.CompletedTask;
    }

    public virtual  bool Contains(TKey key) => Dictionary.ContainsKey(key);

    public virtual Task<bool> ContainsAsync(
        TKey key,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Contains(key));

    public virtual  TType Get(TKey key)
    {
        if (!Dictionary.TryGetValue(key, out var item))
            throw new GetFromStoreException(key);

        return item;
    }

    public virtual Task<TType> GetAsync(
        TKey key,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Get(key));

    public virtual void Remove(TKey key)
    {
        if (!Dictionary.TryRemove(key, out _))
            throw new RemoveFromStoreException(key);
    }

    public virtual Task RemoveAsync(
        TKey key,
        CancellationToken cancellationToken = default)
    {
        Remove(key);
        
        return Task.CompletedTask;
    }
}