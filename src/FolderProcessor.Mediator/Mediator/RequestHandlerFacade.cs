using FolderProcessor.Mediator.Mediator.Abstractions;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator.Mediator;

public class RequestHandlerFacade<TRequest, TResponse> : 
    IRequestHandlerFacade<TRequest, TResponse>
    where TRequest : IRequestFacade<TResponse>
{
    private readonly MyMediator.IRequestHandler<TRequest, TResponse> _handler;

    public RequestHandlerFacade(
        MyMediator.IRequestHandler<TRequest, TResponse> handler)
    {
        _handler = handler;
    }

    public Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken)
    {
        return _handler.Handle(request, cancellationToken);
    }
}