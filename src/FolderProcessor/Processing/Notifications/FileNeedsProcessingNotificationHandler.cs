using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Mediator;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Notifications;

[UsedImplicitly]
public class FileNeedsProcessingNotificationHandler : 
    IDomainEventHandler<FileNeedsProcessingNotification>
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
        try
        {
            _logger.LogDebug("{File} needs processing...", notification.File);

            return _mediator.Send(
                (IProcessFileRequest) new ProcessFileRequest {FileId = notification.File.Id}, 
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "There was an error while processing the file {File}...", 
                notification.File);
        }
        
        return Task.CompletedTask;
    }
}