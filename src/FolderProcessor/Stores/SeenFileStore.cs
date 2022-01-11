using System.Collections.Concurrent;

namespace FolderProcessor.Stores;

public class SeenFileStore : ISeenFileStore
{
    private readonly ConcurrentDictionary<string, FileInfo> _seen = new();
    
    public void Add(string fileName)
    {
        _seen.AddOrUpdate(fileName, v => new FileInfo(v), (_, info) => info);
    }

    public bool Contains(string fileName)
    {
        return _seen.ContainsKey(fileName);
    }

    public void Remove(string fileName)
    {
        _seen.Remove(fileName, out _);
    }
}