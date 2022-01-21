using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Processing.Behaviors;

/// <summary>
/// This behavior moves files to the completed directory once they have completed
/// successfully.
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">The type of the response</typeparam>
[UsedImplicitly]
public class CompletedFileMovingBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, IProcessFileRequest
{
    private readonly IWorkingFileStore _workingStore;
    private readonly ICompletedFileStore _completedStore;
    private readonly ICompletedDirectoryProvider _provider;
    private readonly IFileMover _fileMover;
    private readonly IPublisher _publisher;

    public CompletedFileMovingBehavior(
        IWorkingFileStore workingStore, 
        IFileMover fileMover, 
        ICompletedDirectoryProvider provider, 
        ICompletedFileStore completedStore, 
        IPublisher publisher)
    {
        _workingStore = workingStore;
        _fileMover = fileMover;
        _provider = provider;
        _completedStore = completedStore;
        _publisher = publisher;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        var result = await next();
        
        // Get the file and where to send it.
        var file = new FileRecord(await _workingStore
            .GetAsync(request.FileId, cancellationToken));
        var destination = await _fileMover
            .MoveFileAsync(file, _provider, cancellationToken);

        // Remove it from seen and add it to working
        var newFileLocation = file with {Path = destination};
        
        await Task.WhenAll(
            _completedStore.AddAsync(
                newFileLocation.Id,
                newFileLocation,
                cancellationToken),
            _workingStore.RemoveAsync(
                file.Id,
                cancellationToken));
        await _publisher.Publish(
            new CompletedFileMovedNotification {FileRecord = newFileLocation},
            cancellationToken);

        return result;
    }
}