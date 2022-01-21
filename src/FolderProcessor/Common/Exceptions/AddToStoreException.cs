namespace FolderProcessor.Common.Exceptions;

/// <summary>
/// Thrown when there was an error adding an item to a store.
/// </summary>
public class AddToStoreException : Exception
{
    public AddToStoreException(object key, object item) : 
        base($"Could not add '{item}' with key '{key}' to store.") {}
}