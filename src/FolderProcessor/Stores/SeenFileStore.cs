using System.Collections.Concurrent;
using System.IO.Abstractions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Stores;

/// <summary>
/// Maintains a thread-safe <see cref="ConcurrentDictionary{TKey,TValue}"/> to
/// maintain a collection of all seen files.
/// </summary>
public class SeenFileStore : Store<Guid, IFileRecord>, ISeenFileStore
{
    private readonly ILogger<SeenFileStore> _logger;
    private readonly IFileSystem _fileSystem;

    public SeenFileStore(
        ILogger<SeenFileStore> logger, 
        IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public IFileRecord AddFileRecord(string filePath)
    {
        var fileName = _fileSystem.Path.GetFileName(filePath);
        var record = new FileRecord(filePath, fileName);

        base.Add(record.Id, record);
        _logger.LogDebug("Added {Record} to {Store}", record, nameof(SeenFileStore));
        return record;
    }

    public bool ContainsPath(string filePath)
    {
        return Dictionary.Values.Any(p => p.Path.Equals(filePath));
    }

    public async Task<bool> ContainsPathAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() => ContainsPath(filePath), cancellationToken)
            .ConfigureAwait(false);
    }

    public override void Remove(Guid key)
    {
        _logger.LogInformation("Removing {key} from {Store}", key, nameof(SeenFileStore));
        base.Remove(key);
    }
}