namespace FolderProcessor.Abstractions.Monitoring.Notifiers;

public interface IFileNotifier
{
    /// <summary>
    /// The folder to monitor.
    /// </summary>
    string Folder { get; set; }
}