namespace FolderProcessor.Host.Models;

public class FolderProcessorSettings
{
    public IEnumerable<WatcherSettings> Watchers { get; set; } = null!;
}

public enum WatcherType
{
    Polling,
    FileSystemWatcher
}

public class WatcherSettings
{
    public WatcherType Type { get; set; }
    public string Folder { get; set; } = null!;
}

public class PollingWatcherSettings : WatcherSettings
{
    public int Interval { get; set; }
}