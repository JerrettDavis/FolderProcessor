using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Streams;

public class PolledFileStreamHandlerTests
{
    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldSeeExistingFilesAndNewlyCreatedFiles(
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] ISeenFileStore seenFileStore,
        PolledFileStreamHandler handler)
    {
        // Arrange
        var root = fileSystem.AllDirectories.MinBy(r => r.Length)!;
        var beenSeen = Path.Combine(root, @"Data", "BeenSeen.txt");
        seenFileStore.AddFileRecord(beenSeen);
        var request = new PolledFileStream
        {
            Folder = Path.Combine(root, "Data"),
            Interval = TimeSpan.FromMilliseconds(1) // zoom zoom ;)
        };
        var files = new List<string>
        {
            beenSeen, // This one should be skipped
            Path.Combine(root, "Data", "tmp.xml"),
            Path.Combine(root, "Data", "tmp2.xml"),
            Path.Combine(root, "Data", "tmp.bmp")
        };
        var count = files.Count - 1;
        fileSystem.AddDirectory(request.Folder);
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        // Start watching
        var handle = handler.Handle(request, cancellationTokenSource.Token);
        // Run a task to create some files and cancel after some time
        var filesTask = Task.Run(() =>
            {
                files.ForEach(f => fileSystem.FileSystemWatcherFactory.NewFile(f));
                // This should not be returned in the array since it's a directory
                fileSystem.FileSystemWatcherFactory.NewFile(Path.Combine(root, "Data", "Data2"));
                
                // ReSharper disable once AccessToDisposedClosure
                cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(1000));
            },
            CancellationToken.None);
        var output = new ConcurrentBag<IFileRecord>();
        var toListTask = Task.Run(async () =>
        {
            try
            {
                // ReSharper disable once AccessToDisposedClosure
                await foreach (var item in handle.WithCancellation(cancellationTokenSource.Token))
                {
                    output.Add(item);
                }     
            } catch (OperationCanceledException) {}
            
        }, CancellationToken.None);
        
        // Await completion
        await Task.WhenAll(filesTask, toListTask).ConfigureAwait(false);

        // Assert
        output.Should().NotBeNullOrEmpty();
        output.Should().HaveCount(count);
    }
}