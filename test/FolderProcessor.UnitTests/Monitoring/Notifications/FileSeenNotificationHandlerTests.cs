using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Monitoring.Notifications;
using FolderProcessor.UnitTests.Setup.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Notifications;

public class FileSeenNotificationHandlerTests
{
    [Theory, AutoMoqData]
    public async Task ShouldRun(
        [Frozen] Mock<ILogger<FileSeenNotificationHandler>> logger,
        FileSeenNotificationHandler handler,
        FileSeenNotification notification)
    {
        // Sanity check
        logger.Invocations.Should().BeEmpty();
        
        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        // No idea how to test this really...
        logger.Invocations.Should().NotBeEmpty();
    }
}