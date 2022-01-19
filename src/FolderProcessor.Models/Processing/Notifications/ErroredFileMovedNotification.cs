using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Models.Processing.Notifications;

[PublicAPI]
public class ErroredFileMovedNotification : INotification
{
    public IFileRecord FileRecord { get; set; } = null!;
}