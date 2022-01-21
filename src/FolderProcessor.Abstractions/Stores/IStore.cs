using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Stores
{
    /// <summary>
    /// A contract for a generic object store
    /// </summary>
    /// <typeparam name="TKey">The type of key of the item to store</typeparam>
    /// <typeparam name="TType">The type of the time to store</typeparam>
    [PublicAPI]
    public interface IStore<in TKey, TType>
        where TType : class
    {
        /// <summary>
        /// Adds a file to the store.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <param name="item">The item to add to the store.</param>
        void Add(TKey key, TType item);

        /// <summary>
        /// Asynchronously adds a file to the store.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <param name="item">The item to add to the store.</param>
        /// <param name="cancellationToken">A token allowing you to cancel the task early</param>
        Task AddAsync(TKey key, TType item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns whether the store contains a reference to the given file path.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <returns>True if the store contains the key.</returns>
        bool Contains(TKey key);

        /// <summary>
        /// Returns whether the store contains a reference to the given file path.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <param name="cancellationToken">A token allowing you to cancel the task early</param>
        /// <returns>A task with a true result if the store contains the key.</returns>
        Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the item from the store with the matching key
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <returns>The item with the matching key from the store</returns>
        TType Get(TKey key);

        /// <summary>
        /// Fetches the item from the store with the matching key
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <param name="cancellationToken">A token allowing you to cancel the task early</param>
        /// <returns>The item with the matching key from the store</returns>
        Task<TType> GetAsync(TKey key, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Removes a seen file from the store. Generally this is used after a file
        /// has been removed from the directory.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        void Remove(TKey key);

        /// <summary>
        /// Removes a seen file from the store. Generally this is used after a file
        /// has been removed from the directory.
        /// </summary>
        /// <param name="key">The unique key for the stored item.</param>
        /// <param name="cancellationToken">A token allowing you to cancel the task early</param>
        Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
   
    }
}