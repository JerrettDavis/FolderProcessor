namespace FolderProcessor.Common.Exceptions;

public class AddToStoreException : Exception
{
    public AddToStoreException(object key, object item) : 
        base($"Could not add '{item}' with key '{key}' to store.") {}
}