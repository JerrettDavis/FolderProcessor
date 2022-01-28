namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface IPublisherFacade 
{
    Task Publish(
        object domainEvent, 
        CancellationToken cancellationToken = default);

    Task Publish<TEvent>(
        TEvent domainEvent, 
        CancellationToken cancellationToken = default)
        where TEvent : INotificationFacade;
}