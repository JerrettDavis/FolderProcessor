using FolderProcessor.Models.Processing.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Notifications;

public class FileNeedsProcessingNotificationHandler : 
    INotificationHandler<FileNeedsProcessingNotification>
{
    private readonly ILogger<FileNeedsProcessingNotificationHandler> _logger;

    public FileNeedsProcessingNotificationHandler(
        ILogger<FileNeedsProcessingNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        FileNeedsProcessingNotification notification, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("{File} needs processing...", notification.File);
        return Task.CompletedTask;
    }
}