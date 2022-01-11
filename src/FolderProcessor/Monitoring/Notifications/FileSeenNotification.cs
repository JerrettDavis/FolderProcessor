using FolderProcessor.Models;
using FolderProcessor.Monitoring.Streams;
using MediatR;

namespace FolderProcessor.Monitoring.Notifications;

/// <summary>
/// This notification is thrown by a <see cref="IFileStream"/> when it sees a
/// new file for the first time.
/// </summary>
public class FileSeenNotification : INotification
{
    public FileRecord FileInfo { get; set; } = null!;
}