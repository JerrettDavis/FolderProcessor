using FolderProcessor.Models.Files;
using MediatR;

namespace FolderProcessor.Models.Monitoring.Notifications;

/// <summary>
/// This notification is thrown by a <see cref="IFileStream"/> when it sees a
/// new file for the first time.
/// </summary>
public class FileSeenNotification : INotification
{
    public FileRecord FileInfo { get; set; } = null!;
}