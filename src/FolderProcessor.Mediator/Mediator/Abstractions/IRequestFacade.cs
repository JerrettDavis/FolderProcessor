using MediatR;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface IRequestFacade<out TResponse> : 
    IRequest<TResponse>, 
    MyMediator.IRequest<TResponse>
{
    
}