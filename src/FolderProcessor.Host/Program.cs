using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Processing;
using FolderProcessor.Host;
using FolderProcessor.Host.Processors;
using FolderProcessor.Models.Monitoring.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services
            // Automatically sets up FolderProcessor with settings loaded from IConfiguration
            .AddFolderProcessor(host.Configuration)
            // Since we setup folder processor above, we can just add ad-hoc watchers
            .AddFolderWatcher(() => new PollingFolderWatcherSettings
            {
                Folder = "Data/Child",
                Interval = TimeSpan.FromSeconds(30)
            })
            .AddFolderWatcher(() => new WatcherSettings("Data/Child"))
            .AddFileTypeFilter(".txt")
            .UseStaticWorkingFile("Working")
            .UseStaticCompletedFile("Completed")
            .UseStaticErroredFile("Errored")
            .AddProcessor<LogFileContentProcessor>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();