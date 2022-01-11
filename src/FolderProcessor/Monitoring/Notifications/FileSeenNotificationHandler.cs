using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Notifications;

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
        _logger.LogInformation("File seen at {Path}", notification.FileInfo);
        return Task.CompletedTask;
    }
}