using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using MediatR;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Streams
{
    public class PolledFileStreamHandlerTests
    {
        [Theory, AutoMoqDataWithFileSystem]
        public async Task ShouldSeeExistingFilesAndNewlyCreatedFiles(
            [Frozen] MyMockFileSystem fileSystem,
            [Frozen] ISeenFileStore seenFileStore,
            [Frozen] IPublisher publisher,
            PolledFileStreamHandler handler)
        {
            // Arrange
            var root = fileSystem.AllDirectories.MinBy(r => r.Length);
            Assert.NotNull(root);
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
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            // Start watching
            var handle = handler.Handle(request, cancellationTokenSource.Token);
            // Run a task to create some files and cancel after some time
            await Task.Run(() =>
                {
                    files.ForEach(f => fileSystem.FileSystemWatcherFactory.NewFile(f));
                    // This should not be returned in the array since it's a directory
                    fileSystem.FileSystemWatcherFactory.NewFile(Path.Combine(root, "Data", "Data2"));
                    
                    // Wait some time and then cancel        
                    cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(1000));
                },
                CancellationToken.None);

            // Await completion
            var result = await handle;

            // Assert
            result.Should().NotBeNull();
            Mock.Get(publisher).Verify(p =>
                    p.Publish(It.IsAny<FileSeenNotification>(), It.IsAny<CancellationToken>()),
                Times.Exactly(count));
            
            cancellationTokenSource.Dispose();
        }

        [Theory, AutoMoqDataWithFileSystem]
        public async Task ShouldHandleFileNotFound(
            [Frozen] MyMockFileSystem fileSystem,
            [Frozen] IPublisher publisher,
            PolledFileStreamHandler handler)
        {
            // Arrange
            var root = fileSystem.AllDirectories.MinBy(r => r.Length);
            Assert.NotNull(root);
            var request = new PolledFileStream
            {
                Folder = Path.Combine(root, "Data", "2"),
                Interval = TimeSpan.Zero
            };
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            // Start watching
            var handle = handler
                .Handle(request, cancellationTokenSource.Token);

            // Cancel monitoring
            cancellationTokenSource.CancelAfter(1000);

            await handle;

            // Assert
            Mock.Get(publisher)
                .Verify(p => p.Publish(
                        It.IsAny<DirectoryNotFoundNotification>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            
            cancellationTokenSource.Dispose();
        }
    }    
}

