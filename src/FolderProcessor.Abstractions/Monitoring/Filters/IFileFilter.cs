namespace FolderProcessor.Abstractions.Monitoring.Filters;

/// <summary>
/// Defines a filter used for determining if a file is valid.
/// </summary>
public interface IFileFilter
{
    /// <summary>
    /// Determines if the inputted file is valid
    /// </summary>
    /// <param name="input">The file to check</param>
    /// <param name="cancellationToken">Cancels the operations</param>
    /// <returns>True if the file is valid.</returns>
    Task<bool> IsValid(string input, CancellationToken cancellationToken = default);
}