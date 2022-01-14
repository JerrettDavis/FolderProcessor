using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring;
using FolderProcessor.Host;
using FolderProcessor.Models.Monitoring.Configuration;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Filters;
using MediatR;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddMediatR(typeof(Program), typeof(StreamedFolderWatcher))
            // Automatically sets up FolderProcessor with settings loaded from IConfiguration
            .AddFolderProcessor(host.Configuration)
            // Since we setup folder processor above, we can just add ad-hoc watchers
            .AddFolderWatcher(() => new PollingFolderWatcherSettings
            {
                Folder = "Data/Child",
                Interval = TimeSpan.FromSeconds(30)
            })
            .AddFolderWatcher(() => new WatcherSettings("Data/Child"))
            .AddSingleton<IFileFilter>(s => new FileTypeFileFilter(s.GetService<IFileSystem>()!, new []{ ".txt" }))
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();