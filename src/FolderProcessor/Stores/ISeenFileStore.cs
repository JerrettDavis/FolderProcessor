namespace FolderProcessor.Stores;

public interface ISeenFileStore
{
    Task AddAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> ContainsAsync(string fileName, CancellationToken cancellationToken = default);
    Task RemoveAsync(string fileName, CancellationToken cancellationToken = default);
}