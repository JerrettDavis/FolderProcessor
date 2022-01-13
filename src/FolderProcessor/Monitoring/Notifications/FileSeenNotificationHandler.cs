using FolderProcessor.Models.Monitoring.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Notifications;

[UsedImplicitly]
public class FileSeenNotificationHandler : 
    INotificationHandler<FileSeenNotification>
{
    private readonly ILogger<FileSeenNotificationHandler> _logger;

    public FileSeenNotificationHandler(
        ILogger<FileSeenNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        FileSeenNotification notification, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saw new {Path}", notification.FileInfo);
        return Task.CompletedTask;
    }
}