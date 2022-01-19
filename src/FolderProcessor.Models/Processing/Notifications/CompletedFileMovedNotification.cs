using FolderProcessor.Abstractions.Files;
using MediatR;

namespace FolderProcessor.Models.Processing.Notifications;

public class CompletedFileMovedNotification : INotification
{
    public IFileRecord FileRecord { get; set; } = null!;
}