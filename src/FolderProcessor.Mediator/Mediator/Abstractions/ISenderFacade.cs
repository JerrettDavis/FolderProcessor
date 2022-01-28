namespace FolderProcessor.Mediator.Mediator.Abstractions;

public interface ISenderFacade
{
    Task<TResponse> Send<TResponse>(
        IRequestFacade<TResponse> request, 
        CancellationToken cancellationToken = default);
    
    Task<object?> Send(
        object request, 
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequestFacade<TResponse> request, 
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<object?> CreateStream(
        object request, 
        CancellationToken cancellationToken = default);
}