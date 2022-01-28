using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FolderProcessor.Abstractions.Mediator;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing.Notifications;
using FolderProcessor.Processing;
using FolderProcessor.Processing.Notifications;
using FolderProcessor.UnitTests.Setup.Attributes;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Processing.Notifications;

public class FileNeedsProcessingNotificationHandlerTests
{
    [Theory, AutoMoqData]
    public async Task ShouldSendProcessingRequest(
        [Frozen] IMediator mediator,
        FileRecord fileRecord,
        FileNeedsProcessingNotificationHandler handler)
    {
        // Arrange
        var request = new FileNeedsProcessingNotification
        {
            File = fileRecord
        };

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        Mock.Get(mediator).Verify(m => m.Send(
            It.Is<ProcessFileRequest>(p => p.FileId == fileRecord.Id),
            It.IsAny<CancellationToken>()));
    }
}