using FolderProcessor.Abstractions.Common;
using MediatR;

namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface INotificationFacade : IDomainEvent, INotification
{
    
}