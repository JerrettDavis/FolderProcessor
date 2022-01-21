using System;

namespace FolderProcessor.Common.Exceptions
{
    /// <summary>
    /// Thrown when there's an error fetching an item from a store.
    /// </summary>
    public class GetFromStoreException : Exception
    {
        public GetFromStoreException(object key) : 
            base($"Could not get item with key '{key}' from the store.") {}
    }    
}