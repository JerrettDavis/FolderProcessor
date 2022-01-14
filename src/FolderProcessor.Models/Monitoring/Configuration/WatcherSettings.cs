using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration;

[PublicAPI]
public class WatcherSettings
{
    public WatcherType Type { get; set; }
    public string Folder { get; set; } = null!;

    public WatcherSettings()
    {
        Type = WatcherType.FileSystemWatcher;
    }

    public WatcherSettings(string folder) : this()
    {
        Folder = folder;
    }

    public static WatcherSettings Create(WatcherType type, string folder)
    {
        return type switch
        {
            WatcherType.Polling => new PollingFolderWatcherSettings(folder),
            WatcherType.FileSystemWatcher => new WatcherSettings(folder),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}