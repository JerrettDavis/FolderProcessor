using MediatR;

namespace FolderProcessor.Abstractions.Monitoring.Notifiers;

public interface IFileNotifier : IRequest
{
    /// <summary>
    /// The folder to monitor.
    /// </summary>
    string Folder { get; set; }
}