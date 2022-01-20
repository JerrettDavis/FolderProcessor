using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;
using MediatR.Pipeline;

namespace FolderProcessor.Processing.PostProcessing;

[UsedImplicitly]
public class CompletedFileMovingBehavior<TRequest, TResponse> : 
    IRequestPostProcessor<TRequest, TResponse> 
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

    public async Task Process(
        TRequest request, 
        TResponse response, 
        CancellationToken cancellationToken)
    {
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
    }
}