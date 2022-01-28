using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Streams;
using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Notifications;

/// <summary>
/// This notification is thrown by a <see cref="IFileStream"/> when it sees a
/// new file for the first time.
/// </summary>
[PublicAPI]
public class FileSeenNotification : IDomainEvent
{
    /// <summary>
    /// Information about the seen file.
    /// </summary>
    public IFileRecord FileInfo { get; init; } = null!;
}