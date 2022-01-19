using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Processing;

/// <summary>
/// Defines a processor that performs some action on or with a file.
/// </summary>
public interface IProcessor
{
    /// <summary>
    /// Processes the given file record
    /// </summary>
    /// <param name="fileRecord">The file record to process</param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the asynchronous operation.
    /// </param>
    /// <returns></returns>
    Task Process(
        IFileRecord fileRecord,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Determines if a processor can run for a given file record.
    /// </summary>
    /// <param name="fileRecord">The file record to check</param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the asynchronous operation.
    /// </param>
    /// <returns></returns>
    Task<bool> AppliesAsync(
        IFileRecord fileRecord, 
        CancellationToken cancellationToken = default);
}