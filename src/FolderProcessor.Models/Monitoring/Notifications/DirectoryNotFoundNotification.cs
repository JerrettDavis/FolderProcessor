using MediatR;

namespace FolderProcessor.Models.Monitoring.Notifications;

public class DirectoryNotFoundNotification : INotification
{
    public string Path { get; set; } = null!;
}