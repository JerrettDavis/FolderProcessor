using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Host;
using FolderProcessor.Models.Configuration;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Filters;
using MediatR;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddMediatR(typeof(Program), typeof(StreamedFolderWatcher))
            .AddFolderProcessor(host.Configuration)
            .AddFolderWatcher(() => new PollingWatcherSettings
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