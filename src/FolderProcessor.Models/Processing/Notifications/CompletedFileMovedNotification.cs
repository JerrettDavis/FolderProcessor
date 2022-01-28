using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;

namespace FolderProcessor.Models.Processing.Notifications;

/// <summary>
/// This domain event is published after a completed file has been moved to the
/// 'completed' directory. 
/// </summary>
[PublicAPI]
public class CompletedFileMovedNotification : IDomainEvent
{
    /// <summary>
    /// Information about the file
    /// </summary>
    public IFileRecord FileRecord { get; set; } = null!;
}