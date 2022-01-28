using MediatR;

namespace FolderProcessor.Mediator.Mediator.Abstractions;
using MyMediator = FolderProcessor.Abstractions.Mediator;

public interface IStreamRequestHandlerFacade<in TRequest, out TResponse> :
    IStreamRequestHandler<TRequest, TResponse>
    where TRequest : IStreamRequestFacade<TResponse>
{
    
}