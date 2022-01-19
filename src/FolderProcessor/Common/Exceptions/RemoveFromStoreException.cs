namespace FolderProcessor.Common.Exceptions;

public class RemoveFromStoreException : Exception
{
    public RemoveFromStoreException(object key) : 
        base($"Could not remove {key}") { }
}