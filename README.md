# FolderProcessor

![Main Build Status](https://github.com/JerrettDavis/FolderProcessor/actions/workflows/main.yml/badge.svg)
![Develop Build Status](https://github.com/JerrettDavis/FolderProcessor/actions/workflows/develop.yml/badge.svg)
![Test Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/JerrettDavis/c0d1f93e62b9e0910bffca6a7c0aede0/raw/0cf8241600fb0658e2ca506aee0e6e131443ff41/code-coverage.json)

An unambitious, hostable, extensible, and reusable folder monitoring and file processing framework.

## What is FolderProcessor

FolderProcessor is a two-part framework that aims to make building routing file
processing applications a bit easier. It accomplishes this by giving you a flexible, 
easy-to-use pattern that you can apply over and over to your various processing 
needs. At it's core, FolderProcessor consists of two main domains: `Monitoring` and `Processing`.

`Monitoring` is the activity of continuously checking some source (usually the 
filesystem) for the presence of a file. 

`Processing` is the activity of consuming the emitted files, and performing some
action with them.

Both monitoring and processing happen independently of each other by way of domain
events. You can choose to use either of them in isolation, or in tandem. Also, 
while the system is designed to use files directly on the filesystem, one doesn't
necessary *have* to.


## Using FolderProcessor

*At some point I will get this on nuget. Until then the project will need to be referenced directly*

Presently, the framework is built to work directly with .Net's `ServiceCollection`,
but there's no inherent reason it could not be adapted to work with other IoC containers.

Before we get started, lets take a look at what a simple configuration might 
look like.

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services
            // Adds all the required internal services and the hosted worker service
            .AddFolderProcessor()
            // Creates a new polled folder watcher, monitoring a folder called "Data", 
            // with an interval of 30 seconds
            .AddFolderWatcher(() => new PollingFolderWatcherSettings
            {
                Folder = "Data",
                Interval = TimeSpan.FromSeconds(30)
            })
            // Creates a new file system watcher, monitoring a folder called "Data"
            .AddFolderWatcher(() => new WatcherSettings("Data"))
            // Creates a filter that causes watchers to only emit files ending with ".txt"
            .AddFileTypeFilter(".txt")
            // Moves all working files to a folder called "Working"
            .UseStaticWorkingFile("Working")
            // Moves all completed files to a folder called "Completed"
            .UseStaticCompletedFile("Completed")
            // Moves all errored files to a folder called "Errored"
            .UseStaticErroredFile("Errored")
            // Adds a new file processor
            .AddProcessor<LogFileContentProcessor>();
    })
    .Build();

await host.RunAsync();
```

In the above snippet, you can see that we're creating a simple generic .net host
with a couple additional extension methods. Each of the methods are documented with
what they do. On the last line, you'll see that we're adding a processor called 
`LogFileContentProcessor`, and it looks like:

```csharp
public class LogFileContentProcessor : IProcessor
{
    private readonly ILogger<LogFileContentProcessor> _logger;
    private readonly IFileSystem _fileSystem;

    public LogFileContentProcessor(
        ILogger<LogFileContentProcessor> logger, 
        IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public async Task ProcessAsync(
        IFileRecord fileRecord, 
        CancellationToken cancellationToken = default)
    {
        // This is DANGEROUS. Processors are ran concurrently, and this could cause
        // the file to get locked if multiple processors are attempting to access
        // it simultaneously.
        var content = await _fileSystem.File
            .ReadAllTextAsync(fileRecord.Path, cancellationToken);
        
        _logger.LogInformation("File {File}. Content '{Content}'", fileRecord, content);
    }
    
    public Task<bool> AppliesAsync(
        IFileRecord fileRecord, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
```

Each processor you define contains 2 required methods: `ProcessAsync` and `AppliesAsync`.
In `AppliesAsync`, you add any necessary logic to determine if your particular processor
applies to the input file. The `ProcessAsync` method is where you place any processing
logic. As you may note from the comment in the sample above, processors are ran
concurrently. You will need to utilize caution when attempting to access shared resources.