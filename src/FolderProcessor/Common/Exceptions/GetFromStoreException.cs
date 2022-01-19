namespace FolderProcessor.Common.Exceptions;

public class GetFromStoreException : Exception
{
    public GetFromStoreException(object key) : 
        base($"Could not get item with key '{key}' from the store.") {}
}