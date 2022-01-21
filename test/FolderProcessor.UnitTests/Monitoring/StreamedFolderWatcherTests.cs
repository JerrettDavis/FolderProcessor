using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Processing.Notifications;
using FolderProcessor.Monitoring;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using MediatR;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring
{
    // TODO: FIX
    public class StreamedFolderWatcherTests
    {
        [Theory, AutoMoqDataWithFileStreamHandlers]
        public async Task ShouldCreateAndStartFileStreams(
            [Frozen] IEnumerable<IFileStream> fileStreams,
            [Frozen] IPublisher publisher,
            [Frozen] IMediator mediator,
            StreamedFolderWatcher watcher)
        {
            // Arrange
            using (var cancellationSource = new CancellationTokenSource())
            {
                var fsList = fileStreams.ToList();
        
                // Act
                await Task.Run(() => cancellationSource.CancelAfter(2000), CancellationToken.None);
                await Task.Run(async () => await watcher.StartAsync(cancellationSource.Token), CancellationToken.None);

                // Assert
                // Mock.Get(mediator)
                //     .Verify(m => m.CreateStream(It.IsAny<IFileStream>(), It.IsAny<CancellationToken>()),
                //         Times.Exactly(fsList.Count));
                // Mock.Get(publisher)
                //     .Verify(p => p.Publish(It.IsAny<FileNeedsProcessingNotification>(), It.IsAny<CancellationToken>()),
                //         Times.Exactly(MockFiles.Files.Count() * fsList.Count));    
            }
        } 
    }    
}

