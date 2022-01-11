using FolderProcessor.Models;
using MediatR;

namespace FolderProcessor.Processing.Notifications;

public class FileNeedsProcessingNotification : INotification
{
    public FileRecord File { get; set; } = null!;
}