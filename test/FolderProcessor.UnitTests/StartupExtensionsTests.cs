using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Monitoring.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FolderProcessor.UnitTests;

public class StartupExtensionsTests
{
    [Fact]
    public void AddFolderProcessor_RegistersFrameworkAndApplicationHandlers()
    {
        var services = new ServiceCollection();

        services.AddFolderProcessor(typeof(StartupExtensionsTests).Assembly);

        Assert.Contains(services, service =>
            service.ServiceType == typeof(INotificationHandler<FileSeenNotification>) &&
            service.ImplementationType == typeof(FileSeenNotificationHandler));
        Assert.Contains(services, service =>
            service.ServiceType == typeof(INotificationHandler<TestNotification>) &&
            service.ImplementationType == typeof(TestNotificationHandler));
    }
}

public class TestNotification : INotification
{
}

public class TestNotificationHandler : INotificationHandler<TestNotification>
{
    public Task Handle(TestNotification notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
