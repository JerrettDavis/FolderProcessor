using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Models.Monitoring.Notifications
{
    /// <summary>
    /// This domain event is published whenever a given directory to monitor is not
    /// found. This can happen if the directory is deleted after watcher initialization.
    /// </summary>
    [PublicAPI]
    public class DirectoryNotFoundNotification : INotification
    {
        /// <summary>
        /// The path of the directory
        /// </summary>
        public string Path { get; set; }
    }    
}

