using FolderProcessor.Abstractions.Files;
using MediatR;

namespace FolderProcessor.Processing.Notifications;

public class FileNeedsProcessingNotification : INotification
{
    public IFileRecord File { get; set; } = null!;
}