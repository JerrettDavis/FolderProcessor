using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Notifications;

[UsedImplicitly]
public class FileNeedsProcessingNotificationHandler : 
    INotificationHandler<FileNeedsProcessingNotification>
{
    private readonly ILogger<FileNeedsProcessingNotificationHandler> _logger;
    private readonly IMediator _mediator;
    
    public FileNeedsProcessingNotificationHandler(
        ILogger<FileNeedsProcessingNotificationHandler> logger, 
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public Task Handle(
        FileNeedsProcessingNotification notification, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("{File} needs processing...", notification.File);

        return _mediator.Send(
            new ProcessFileRequest {FileId = notification.File.Id}, 
            cancellationToken);
    }
}