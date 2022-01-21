using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Notifications
{
    /// <summary>
    /// This handler responses to a <see cref="FileSeenNotification"/>. Currently,
    /// it just logs the file.
    /// </summary>
    [UsedImplicitly]
    public class FileSeenNotificationHandler : 
        INotificationHandler<FileSeenNotification>
    {
        private readonly ILogger<FileSeenNotificationHandler> _logger;
        private readonly IEnumerable<IFileFilter> _filters;
        private readonly IPublisher _publisher;

        public FileSeenNotificationHandler(
            ILogger<FileSeenNotificationHandler> logger, 
            IEnumerable<IFileFilter> filters, 
            IPublisher publisher)
        {
            _logger = logger;
            _filters = filters;
            _publisher = publisher;
        }

        public async Task Handle(
            FileSeenNotification notification, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saw new {Path}", notification.FileInfo);
            
            var file = notification.FileInfo;
            var satisfiedFilters = await _filters
                .ToAsyncEnumerable()
                .AllAwaitAsync(async f => 
                        await f.IsValid(file.Path, cancellationToken),
                    cancellationToken);
            if (!satisfiedFilters) return;

            _logger.LogDebug("Incoming {File}", file);
            await _publisher.Publish(
                new FileNeedsProcessingNotification {File = file},
                cancellationToken);
        }
    }    
}