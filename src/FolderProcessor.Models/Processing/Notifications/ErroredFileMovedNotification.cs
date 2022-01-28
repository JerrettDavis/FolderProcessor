using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;

namespace FolderProcessor.Models.Processing.Notifications;

/// <summary>
/// This domain event is published when a file that errored during processing
/// has been successfully moved to the 'errored' directory.
/// </summary>
[PublicAPI]
public class ErroredFileMovedNotification : IDomainEvent
{
    /// <summary>
    /// Information about the moved file.
    /// </summary>
    public IFileRecord FileRecord { get; set; } = null!;
}