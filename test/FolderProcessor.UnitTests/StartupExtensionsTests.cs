using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Models.Monitoring.Notifications;
using FolderProcessor.Monitoring.Notifications;
using MediatR;
using MediatR.Pipeline;
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
        Assert.Contains(services, service =>
            service.ServiceType == typeof(IRequestPreProcessor<TestRequest>) &&
            service.ImplementationType == typeof(TestPreProcessor));
        Assert.Contains(services, service =>
            service.ServiceType == typeof(IRequestPostProcessor<TestRequest, string>) &&
            service.ImplementationType == typeof(TestPostProcessor));
        Assert.Single(services, service =>
            service.ServiceType == typeof(IPipelineBehavior<,>) &&
            service.ImplementationType == typeof(RequestPreProcessorBehavior<,>));
        Assert.Single(services, service =>
            service.ServiceType == typeof(IPipelineBehavior<,>) &&
            service.ImplementationType == typeof(RequestPostProcessorBehavior<,>));
        Assert.Single(services, service =>
            service.ServiceType == typeof(IPipelineBehavior<,>) &&
            service.ImplementationType == typeof(RequestExceptionActionProcessorBehavior<,>));
        Assert.Single(services, service =>
            service.ServiceType == typeof(IPipelineBehavior<,>) &&
            service.ImplementationType == typeof(RequestExceptionProcessorBehavior<,>));
        Assert.Equal(
            new[]
            {
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
                typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestExceptionProcessorBehavior<,>)
            },
            services.Where(service => service.ServiceType == typeof(IPipelineBehavior<,>))
                .Select(service => service.ImplementationType));
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

public class TestRequest : IRequest<string>
{
}

public class TestPreProcessor : IRequestPreProcessor<TestRequest>
{
    public Task Process(TestRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
}

public class TestPostProcessor : IRequestPostProcessor<TestRequest, string>
{
    public Task Process(TestRequest request, string response, CancellationToken cancellationToken) => Task.CompletedTask;
}

public class TestExceptionHandler : IRequestExceptionHandler<TestRequest, string, InvalidOperationException>
{
    public Task Handle(TestRequest request, InvalidOperationException exception,
        RequestExceptionHandlerState<string> state, CancellationToken cancellationToken) => Task.CompletedTask;
}

public class TestExceptionAction : IRequestExceptionAction<TestRequest, InvalidOperationException>
{
    public Task Execute(TestRequest request, InvalidOperationException exception,
        CancellationToken cancellationToken) => Task.CompletedTask;
}
