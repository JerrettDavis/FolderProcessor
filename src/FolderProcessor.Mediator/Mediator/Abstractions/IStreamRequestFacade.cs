using MediatR;

namespace FolderProcessor.Mediator.Mediator.Abstractions;
using MyMediator = FolderProcessor.Abstractions.Mediator;

public interface IStreamRequestFacade<out TResponse> : 
    IStreamRequest<TResponse>,
    MyMediator.IStreamRequest<TResponse>
{
    
}