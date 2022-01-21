using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Models.Monitoring.Notifications;
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
}