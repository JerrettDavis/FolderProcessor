using System.IO.Abstractions;
using FolderProcessor.Host;
using FolderProcessor.Host.Extensions;
using FolderProcessor.Host.Models;
using FolderProcessor.Monitoring;
using FolderProcessor.Stores;
using MediatR;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        var settings = new FolderProcessorSettings();
        host.Configuration.GetSection("FolderProcessor").Bind(settings);
        
        services.AddMediatR(typeof(Program), typeof(StreamedFolderWatcher))
            .AddSingleton<ISeenFileStore, SeenFileStore>()
            .AddSingleton<StreamedFolderWatcher>()
            .AddFolderWatchers(settings.Watchers)
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();