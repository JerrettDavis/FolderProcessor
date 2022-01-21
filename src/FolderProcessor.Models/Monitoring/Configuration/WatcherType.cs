using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration;

/// <summary>
/// The preconfigured watcher types for the application
/// </summary>
[PublicAPI]
public enum WatcherType
{
    Polling,
    FileSystemWatcher
}