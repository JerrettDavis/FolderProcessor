using MediatR;

namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface IRequestHandlerFacade<in TRequest, TResponse> : 
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequestFacade<TResponse>
{
    
}