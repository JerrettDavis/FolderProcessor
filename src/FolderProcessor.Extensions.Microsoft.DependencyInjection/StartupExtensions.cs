using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Configuration;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.Stores;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection;

[PublicAPI]
public static class StartupExtensions
{
    private const string SettingsString = "FolderProcessor";
    private const string WatchersString = "Watchers";
    private const string TypeString = "Type";
    private const string FolderString = "Folder";
    private const string IntervalString = "Interval";
    
    public static IServiceCollection AddFolderProcessor(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<ISeenFileStore, SeenFileStore>()
            .AddSingleton<StreamedFolderWatcher>()
            .AddFolderWatchers(configuration);

    private static IServiceCollection AddFolderWatchers(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(SettingsString);
        var watchers = settingsSection.GetSection(WatchersString);
        
        foreach (var item in watchers.GetChildren())
        {
            var type = item.GetValue<WatcherType>(TypeString);
            var folder = item.GetValue<string>(FolderString);
            var watcher = WatcherSettings.Create(type, folder);
            if (watcher is PollingWatcherSettings watcherSettings)
                watcherSettings.Interval = TimeSpan
                    .FromMilliseconds(item.GetValue<int>(IntervalString));

            services.AddFolderWatcher(() => watcher);
        }

        return services;
    }

    public static IServiceCollection AddFolderWatcher(
        this IServiceCollection services,
        Func<WatcherSettings> options)
    {
        var settings = options();
        return settings.Type switch
        {
            WatcherType.Polling => AddPollingWatcher(services, _ => (PollingWatcherSettings) settings),
            WatcherType.FileSystemWatcher => AddFileSystemWatcher(services, _ => settings),
            _ => throw new ArgumentOutOfRangeException(nameof(options), settings.Type, null)
        };
    }

    public static IServiceCollection AddPollingWatcher(
        this IServiceCollection services,
        Func<PollingWatcherSettings, PollingWatcherSettings> options)
    {
        var settings = PrepareWatcherEnvironment(options);
        var watcher = new PolledFileStream
        {
            Folder = settings.Folder,
            Interval = settings.Interval
        };

        return services.AddSingleton<IFileStream>(watcher);
    }

    public static IServiceCollection AddFileSystemWatcher(
        this IServiceCollection services,
        Func<WatcherSettings, WatcherSettings> options)
    {
        var settings = PrepareWatcherEnvironment(options);
        var watcher = new FileSystemStream {Folder = settings.Folder};

        return services.AddSingleton<IFileStream>(watcher);
    }

    private static T PrepareWatcherEnvironment<T>(
        Func<T,T> options) where T : WatcherSettings
    {
        var settings = options(Activator.CreateInstance<T>());
        Directory.CreateDirectory(settings.Folder);

        return settings;
    }
}