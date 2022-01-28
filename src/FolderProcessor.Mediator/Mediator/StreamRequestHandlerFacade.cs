using FolderProcessor.Mediator.Mediator.Abstractions;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator.Mediator;

public class StreamRequestHandlerFacade<TRequest, TResponse> : 
    IStreamRequestHandlerFacade<TRequest, TResponse>
    where TRequest : IStreamRequestFacade<TResponse>
{
    private readonly MyMediator.IStreamRequestHandler<TRequest, TResponse> _handler;

    public StreamRequestHandlerFacade(
        MyMediator.IStreamRequestHandler<TRequest, TResponse> handler)
    {
        _handler = handler;
    }

    public IAsyncEnumerable<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken)
    {
        return _handler.Handle(request, cancellationToken);
    }
}