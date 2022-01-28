using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Processing.Notifications;

/// <summary>
/// This domain event is published when a new file is encountered that needs processing. 
/// </summary>
public class FileNeedsProcessingNotification : IDomainEvent
{
    /// <summary>
    /// Information about the file that needs processing.
    /// </summary>
    public IFileRecord File { get; init; } = null!;
}