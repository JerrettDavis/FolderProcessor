// ReSharper disable once CheckNamespace
namespace FolderProcessor.Host.Models;

public enum WatcherType
{
    Polling,
    FileSystemWatcher
}

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
            WatcherType.Polling => new PollingWatcherSettings(folder),
            WatcherType.FileSystemWatcher => new WatcherSettings(folder),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public class PollingWatcherSettings : WatcherSettings
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);

    public PollingWatcherSettings()
    {
        Type = WatcherType.Polling;
    }

    public PollingWatcherSettings(string folder) : this()
    {
        Folder = folder;
    }
}