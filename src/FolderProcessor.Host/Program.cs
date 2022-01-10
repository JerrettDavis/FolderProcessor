using FolderProcessor.Host;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Watchers;
using FolderProcessor.Stores;
using MediatR;
using FileSystemWatcher = FolderProcessor.Monitoring.Watchers.FileSystemWatcher;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMediatR(typeof(Program), typeof(PollingWatcher));

        services.AddSingleton<ISeenFileStore, SeenFileStore>();
        services.AddSingleton<FolderWatcher>();
        
        var path = Path.Combine(Environment.CurrentDirectory, "Data");
        Directory.CreateDirectory(path);
        services.AddSingleton<IWatcher, FileSystemWatcher>(s =>
            new FileSystemWatcher(
                path,
                s.GetService<ISeenFileStore>()!,
                s.GetService<IPublisher>()!,
                s.GetService<ILogger<FileSystemWatcher>>()!
                ));
        services.AddSingleton<IWatcher, PollingWatcher>(s =>
            new PollingWatcher(
                path, 
                TimeSpan.FromSeconds(30), 
                s.GetService<ISeenFileStore>()!,
                s.GetService<IPublisher>()!,
                s.GetService<ILogger<PollingWatcher>>()!)
        );
        var otherPath = Path.Combine(path, "Child");
        Directory.CreateDirectory(otherPath);
        services.AddSingleton<IWatcher, PollingWatcher>(s =>
            new PollingWatcher(
                otherPath, 
                TimeSpan.FromSeconds(30), 
                s.GetService<ISeenFileStore>()!,
                s.GetService<IPublisher>()!,
                s.GetService<ILogger<PollingWatcher>>()!)
        );
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();