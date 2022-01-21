using System;
using JetBrains.Annotations;

namespace FolderProcessor.Models.Monitoring.Configuration
{
    /// <summary>
    /// The generic settings used to configure watchers
    /// </summary>
    [PublicAPI]
    public class WatcherSettings
    {
        /// <summary>
        /// The type of watcher to configure
        /// </summary>
        public WatcherType Type { get; set; }
    
        /// <summary>
        /// The folder to monitor
        /// </summary>
        public string Folder { get; set; }

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
            switch (type)
            {
                case WatcherType.Polling:
                    return new PollingFolderWatcherSettings(folder);
                case WatcherType.FileSystemWatcher:
                    return new WatcherSettings(folder);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }    
}

