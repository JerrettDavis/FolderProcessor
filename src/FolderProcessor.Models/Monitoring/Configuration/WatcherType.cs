using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration;

[PublicAPI]
public enum WatcherType
{
    Polling,
    FileSystemWatcher
}