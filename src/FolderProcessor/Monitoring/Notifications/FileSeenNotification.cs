using FolderProcessor.Models;
using MediatR;

namespace FolderProcessor.Monitoring.Notifications;

public class FileSeenNotification : INotification
{
    public FileRecord FileInfo { get; set; }
}