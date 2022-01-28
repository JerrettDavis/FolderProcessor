using MediatR;

namespace FolderProcessor.Abstractions.Common;

public interface IDomainEventHandler<in TEvent> : 
    INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}