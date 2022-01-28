using MediatR;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface IMediatorFacade :
    ISenderFacade,
    IPublisherFacade
{
}