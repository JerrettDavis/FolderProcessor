namespace FolderProcessor.Abstractions.Providers;

public interface IDirectoryProvider
{
    /// <summary>
    /// Different generators can be applied to files to rename them, change their
    /// extension, or otherwise influence the file destination. This method applies
    /// those rules and returns the final file destination.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetFileDestinationPathAsync(
        string filePath, 
        CancellationToken cancellationToken = default);
}