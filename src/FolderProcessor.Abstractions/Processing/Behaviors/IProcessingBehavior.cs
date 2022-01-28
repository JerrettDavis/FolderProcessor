namespace FolderProcessor.Abstractions.Processing.Behaviors;

public delegate Task<IProcessFileResult> RequestHandlerDelegate();

public interface IProcessingBehavior<in TRequest>
    where TRequest : IProcessFileRequest
{
#pragma warning disable CA1068
    Task<IProcessFileResult> Handle(
#pragma warning restore CA1068
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate next);
}