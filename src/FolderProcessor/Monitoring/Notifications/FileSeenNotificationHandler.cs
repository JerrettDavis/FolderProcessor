using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Notifications;

public class FileSeenNotificationHandler : 
    INotificationHandler<FileSeenNotification>
{
    private readonly IMediator _mediator;
    private readonly ILogger<FileSeenNotificationHandler> _logger;

    public FileSeenNotificationHandler(
        IMediator mediator, 
        ILogger<FileSeenNotificationHandler> logger)
    {
        _mediator = mediator;
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