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
    public class FileSystemStreamHandlerTests
    {
        [Theory, AutoMoqDataWithFileSystem]
        public async Task ShouldSeeNewlyCreatedFilesButIgnoreDirectories(
            [Frozen] MyMockFileSystem fileSystem,
            [Frozen] ISeenFileStore seenFileStore,
            [Frozen] IPublisher publisher,
            FileSystemStreamHandler handler)
        {
            // Arrange
            var root = fileSystem.AllDirectories.OrderBy(r => r.Length).First();
            Assert.NotNull(root);
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
            var cancellationTokenSource = new CancellationTokenSource();
            
            // Act
            // Start watching
            // ReSharper disable once AccessToDisposedClosure
            var handleTask = handler
                .Handle(request, cancellationTokenSource.Token);
            
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
            result.Should().NotBeNull();
            Mock.Get(publisher).Verify(p => 
                p.Publish(It.IsAny<FileSeenNotification>(), It.IsAny<CancellationToken>()),
                Times.Exactly(count));
            
            cancellationTokenSource.Dispose();
        }

        [Theory, AutoMoqDataWithFileSystem]
        public async Task ShouldHandleFileNotFound(
            [Frozen] MyMockFileSystem fileSystem,
            [Frozen] ISeenFileStore fileStore,
            FileSystemStreamHandler handler)
        {
            // Arrange
            var root = fileSystem.AllDirectories.OrderBy(r => r.Length).First();
            Assert.NotNull(root);
            var request = new FileSystemStream { Folder = Path.Combine(root, "Data", "2") };
            var cancellationTokenSource = new CancellationTokenSource();
            var fileName = $"{Guid.NewGuid()}.xml";
            // Act
            // Start watching
            var handle = handler
                .Handle(request, cancellationTokenSource.Token);
            
            fileSystem.FileSystemWatcherFactory.NewFileEvent(Path.Combine(root, @"Data",fileName));
            
            // Cancel monitoring
            cancellationTokenSource.Cancel();
            
            await handle;
            var result = await fileStore
                .ContainsPathAsync(Path.Combine(root, @"Data", fileName), CancellationToken.None); 

            // Assert
            result.Should().BeFalse();
            
            cancellationTokenSource.Dispose();
        }
    }    
}

