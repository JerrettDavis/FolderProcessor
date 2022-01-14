using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration;

[PublicAPI]
public class PollingFolderWatcherSettings : WatcherSettings
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);

    public PollingFolderWatcherSettings()
    {
        Type = WatcherType.Polling;
    }

    public PollingFolderWatcherSettings(string folder) : this()
    {
        Folder = folder;
    }
}