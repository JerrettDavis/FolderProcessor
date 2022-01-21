using FolderProcessor.Abstractions.Files;
using MediatR;

namespace FolderProcessor.Models.Processing.Notifications
{
    /// <summary>
    /// This domain event is published when a new file is encountered that needs processing. 
    /// </summary>
    public class FileNeedsProcessingNotification : INotification
    {
        /// <summary>
        /// Information about the file that needs processing.
        /// </summary>
        public IFileRecord File { get; set; }
    }    
}

