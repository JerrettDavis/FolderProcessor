using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FolderProcessor.Host.Models;

[PublicAPI]
public enum WatcherType
{
    Polling,
    FileSystemWatcher
}

[PublicAPI]
public class WatcherSettings
{
    public WatcherType Type { get; set; }
    public string Folder { get; set; } = null!;

    protected WatcherSettings()
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

[PublicAPI]
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