using FolderProcessor.Abstractions.Processing;
using MediatR.Pipeline;

namespace FolderProcessor.Processing.PreProcessing;

public class WorkingFileMovingBehavior<TRequest> :
    IRequestPreProcessor<TRequest> where TRequest : IProcessor
{
    public Task Process(
        TRequest request, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}