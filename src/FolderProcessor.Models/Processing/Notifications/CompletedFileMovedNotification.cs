using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Models.Processing.Notifications;

/// <summary>
/// This domain event is published after a completed file has been moved to the
/// 'completed' directory. 
/// </summary>
[PublicAPI]
public class CompletedFileMovedNotification : INotification
{
    /// <summary>
    /// Information about the file
    /// </summary>
    public IFileRecord FileRecord { get; set; } = null!;
}