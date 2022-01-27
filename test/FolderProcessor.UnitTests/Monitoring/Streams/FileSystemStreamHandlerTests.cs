using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Streams;

public class FileSystemStreamHandlerTests
{
    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldSeeNewlyCreatedFilesButIgnoreDirectories(
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] ISeenFileStore seenFileStore,
        FileSystemStreamHandler handler)
    {
        // Arrange
        var root = fileSystem.AllDirectories.MinBy(r => r.Length)!;
        var beenSeen = Path.Combine(root, @"Data","BeenSeen.txt");
        seenFileStore.AddFileRecord(beenSeen);
        var request = new FileSystemStream { Folder = Path.Combine(root,"Data")};
        var files = new List<string>
        {
            beenSeen, // This one should be skipped
            Path.Combine(root, "Data","tmp.xml"),
            Path.Combine(root, "Data","tmp2.xml"),
            Path.Combine(root, "Data","tmp.bmp")
        };
        var count = files.Count - 1; 
        fileSystem.AddDirectory(request.Folder);
        using var cancellationTokenSource = new CancellationTokenSource();
        
        // Act
        // Start watching
        // ReSharper disable once AccessToDisposedClosure
        var handleTask = handler
            .Handle(request, cancellationTokenSource.Token)
            .ToListAsync(CancellationToken.None);
        
        // Run a background task to create some files
        await Task.Run(() =>
            {
                files.ForEach(f => fileSystem.FileSystemWatcherFactory.NewFile(f));
                // This should not be returned in the array since it's a directory
                fileSystem.FileSystemWatcherFactory.NewFile(Path.Combine(root,@"Data","Data2"));
                
                // Cancel monitoring
                cancellationTokenSource.CancelAfter(1000);
            }, 
            CancellationToken.None);

        // Await completion
        var result = await handleTask;

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(count);
    }

    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldHandleFileNotFound(
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] ISeenFileStore fileStore,
        FileSystemStreamHandler handler)
    {
        // Arrange
        var root = fileSystem.AllDirectories.MinBy(r => r.Length)!;
        var request = new FileSystemStream { Folder = Path.Combine(root, "Data", "2") };
        using var cancellationTokenSource = new CancellationTokenSource();
        var fileName = $"{Guid.NewGuid()}.xml";
        // Act
        // Start watching
        var handle = handler
            .Handle(request, cancellationTokenSource.Token)
            .ToListAsync(CancellationToken.None);
        
        fileSystem.FileSystemWatcherFactory.NewFileEvent(Path.Combine(root, @"Data",fileName));
        
        // Cancel monitoring
        cancellationTokenSource.Cancel();
        
        await handle;
        var result = await fileStore
            .ContainsPathAsync(Path.Combine(root, @"Data", fileName), CancellationToken.None); 

        // Assert
        result.Should().BeFalse();
    }
}