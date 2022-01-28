using MediatR;

namespace FolderProcessor.Abstractions.Processing.Behaviors;

public interface IProcessingBehavior<in TRequest> : 
    IPipelineBehavior<TRequest, IProcessFileResult> 
    where TRequest : IProcessFileRequest
{
}