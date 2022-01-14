namespace FolderProcessor.Stores;

/// <summary>
/// Maintains a thread-sfe collection of all files seen by various watchers
/// </summary>
public interface ISeenFileStore
{
    /// <summary>
    /// Adds a file to the store.
    /// </summary>
    /// <param name="fileName">The path of the seen file.</param>
    void Add(string fileName);
    
    /// <summary>
    /// Returns whether the store contains a reference to the given file path.
    /// </summary>
    /// <param name="fileName">The path of the seen file.</param>
    /// <returns></returns>
    bool Contains(string fileName);
    
    /// <summary>
    /// Removes a seen file from the store. Generally this is used after a file
    /// has been removed from the directory.
    /// </summary>
    /// <param name="fileName"></param>
    void Remove(string fileName);
}