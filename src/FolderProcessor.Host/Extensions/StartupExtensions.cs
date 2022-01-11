using FolderProcessor.Host.Models;
using FolderProcessor.Monitoring.Streams;

namespace FolderProcessor.Host.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddFolderWatchers(
        this IServiceCollection services,
        IEnumerable<WatcherSettings> settings)
    {
        foreach (var watcher in settings)
        {
            Directory.CreateDirectory(watcher.Folder);
            services.AddSingleton(_ => GetFileStream(watcher));
        }

        return services;
    }
    
    private static IFileStream GetFileStream(WatcherSettings settings)
    {
        switch (settings.Type)
        {
            case WatcherType.Polling:
                var cast = settings as PollingWatcherSettings;
                return new PolledFileStream
                { 
                    Folder = settings.Folder, 
                    Interval = TimeSpan.FromMilliseconds(cast?.Interval ?? 30000) 
                };
            case WatcherType.FileSystemWatcher:
                return new FileSystemStream { Folder = settings.Folder };
            default:
                throw new ArgumentOutOfRangeException(nameof(settings), settings.Type, null);
        }
    }
}