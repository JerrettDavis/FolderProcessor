namespace FolderProcessor.Common.Exceptions;

/// <summary>
/// Thrown when there's an issue removing an item from a store.
/// </summary>
public class RemoveFromStoreException : Exception
{
    public RemoveFromStoreException(object key) : 
        base($"Could not remove {key}") { }
}