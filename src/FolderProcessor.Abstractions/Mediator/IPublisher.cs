using FolderProcessor.Abstractions.Common;

namespace FolderProcessor.Abstractions.Mediator;

public interface IPublisher
{
    Task Publish(
        object domainEvent, 
        CancellationToken cancellationToken = default);

    Task Publish<TEvent>(
        TEvent domainEvent, 
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}