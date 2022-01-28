using FolderProcessor.Abstractions.Common;
using FolderProcessor.Mediator.Mediator.Abstractions;
using MediatR;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator.Mediator;

public class MediatorFacade :
    MyMediator.IMediator
{
    private readonly IMediator _mediator;

    public MediatorFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Publish(
        object domainEvent, 
        CancellationToken cancellationToken = default) =>
        _mediator.Publish(domainEvent, cancellationToken);

    public Task Publish<TEvent>(
        TEvent domainEvent, 
        CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent =>
        _mediator.Publish(domainEvent, cancellationToken);

    public Task<TResponse> Send<TResponse>(MyMediator.IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(MyMediator.IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}