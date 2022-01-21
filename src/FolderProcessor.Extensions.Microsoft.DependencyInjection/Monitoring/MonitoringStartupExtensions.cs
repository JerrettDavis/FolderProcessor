using System;
using System.IO;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Monitoring.Configuration;
using FolderProcessor.Monitoring.Streams;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring
{
    /// <summary>
    /// A set of startup extensions to add Folder Monitoring to an application
    /// </summary>
    [PublicAPI]
    public static class MonitoringStartupExtensions
    {
        private const string SettingsString = "FolderProcessor";
        private const string WatchersString = "Watchers";
        private const string TypeString = "Type";
        private const string FolderString = "Folder";
        private const string IntervalString = "Interval";
        
        /// <summary>
        /// Identifies and adds all the folder watchers defined in <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to use for registration</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing monitor settings</param>
        /// <returns>The updated <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddFolderWatchers(
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
                if (watcher is PollingFolderWatcherSettings watcherSettings)
                    watcherSettings.Interval = TimeSpan
                        .FromMilliseconds(item.GetValue<int>(IntervalString));

                services.AddFolderWatcher(() => watcher);
            }

            return services;
        }

        /// <summary>
        /// Adds a new Folder Watcher to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to use for registration</param>
        /// <param name="options">The options to use for configuring the watcher.</param>
        /// <returns>The updated <see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the settings contain an unknown watcher type</exception>
        public static IServiceCollection AddFolderWatcher(
            this IServiceCollection services,
            Func<WatcherSettings> options)
        {
            var settings = options();
            switch (settings.Type)
            {
                case WatcherType.Polling:
                    return AddPollingWatcher(services, _ => (PollingFolderWatcherSettings) settings);
                case WatcherType.FileSystemWatcher:
                    return AddFileSystemWatcher(services, _ => settings);
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), settings.Type, null);
            }
        }

        /// <summary>
        /// Adds a <see cref="PolledFileStream"/> to the application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to use for registration</param>
        /// <param name="options">The options to use for configuring the new watcher</param>
        /// <returns>The updated <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPollingWatcher(
            this IServiceCollection services,
            Func<PollingFolderWatcherSettings, PollingFolderWatcherSettings> options)
        {
            var settings = PrepareWatcherEnvironment(options);
            var watcher = new PolledFileStream
            {
                Folder = settings.Folder,
                Interval = settings.Interval
            };

            return services.AddSingleton<IFileStream>(watcher);
        }

        /// <summary>
        /// Adds a new <see cref="FileSystemStream"/> to the application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to use for registration</param>
        /// <param name="options">The options to use for configuring the new watcher</param>
        /// <returns></returns>
        public static IServiceCollection AddFileSystemWatcher(
            this IServiceCollection services,
            Func<WatcherSettings, WatcherSettings> options)
        {
            var settings = PrepareWatcherEnvironment(options);
            var watcher = new FileSystemStream {Folder = settings.Folder};

            return services.AddSingleton<IFileStream>(watcher);
        }

        /// <summary>
        /// Creates the specified type of <see cref="WatcherSettings"/> and ensures
        /// the directory specified within it exists.
        /// </summary>
        /// <param name="options">The function to build the settings</param>
        /// <typeparam name="T">The resulting type of the settings</typeparam>
        /// <returns>The newly created settings instance</returns>
        private static T PrepareWatcherEnvironment<T>(
            Func<T,T> options) where T : WatcherSettings
        {
            var settings = options(Activator.CreateInstance<T>());
            Directory.CreateDirectory(settings.Folder);

            return settings;
        }
    }    
}

