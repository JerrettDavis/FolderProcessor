using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Models.Monitoring.Notifications;

[PublicAPI]
public class DirectoryNotFoundNotification : INotification
{
    public string Path { get; init; } = null!;
}