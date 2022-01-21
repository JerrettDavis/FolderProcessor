using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration;

/// <summary>
/// Settings used to configure polling file watchers
/// </summary>
[PublicAPI]
public class PollingFolderWatcherSettings : WatcherSettings
{
    /// <summary>
    /// How long to wait in between checks for new files
    /// </summary>
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