using FolderProcessor.Host;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.Stores;
using MediatR;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMediatR(typeof(Program), typeof(StreamedFolderWatcher));

        services.AddSingleton<ISeenFileStore, SeenFileStore>();
        services.AddSingleton<StreamedFolderWatcher>();

        var path = Path.Combine(Environment.CurrentDirectory, "Data");
        var otherPath = Path.Combine(path, "Child");
        
        Directory.CreateDirectory(path);
        Directory.CreateDirectory(otherPath);
        
        services.AddSingleton<IFileStream>(_ => new PolledFileStream { Folder = path, Interval = TimeSpan.FromSeconds(30)});
        services.AddSingleton<IFileStream>(_ => new PolledFileStream { Folder = otherPath, Interval = TimeSpan.FromSeconds(30) });
        // services.AddSingleton<IFileStream>(_ => new FileSystemStream { Folder = path });
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();