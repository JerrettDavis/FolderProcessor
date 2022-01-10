namespace FolderProcessor.Stores;

public interface ISeenFileStore
{
    void Add(string fileName);
    bool Contains(string fileName);
    void Remove(string fileName);
}