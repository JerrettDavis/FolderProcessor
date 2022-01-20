using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Behaviors;

[UsedImplicitly]
public class ErroredFileMovingBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, IProcessFileRequest
{
    private readonly IWorkingFileStore _workingStore;
    private readonly IErroredFileStore _erroredStore;
    private readonly IErroredDirectoryProvider _provider;
    private readonly IFileMover _fileMover;
    private readonly ILogger<ErroredFileMovingBehavior<TRequest, TResponse>> _logger;
    private readonly IPublisher _publisher;

    public ErroredFileMovingBehavior(
        IWorkingFileStore workingStore, 
        IErroredFileStore erroredStore, 
        IFileMover fileMover, 
        ILogger<ErroredFileMovingBehavior<TRequest, TResponse>> logger, 
        IPublisher publisher, 
        IErroredDirectoryProvider provider)
    {
        _workingStore = workingStore;
        _erroredStore = erroredStore;
        _fileMover = fileMover;
        _logger = logger;
        _publisher = publisher;
        _provider = provider;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            // Get the file and where to send it.
            var file = new FileRecord(await _workingStore
                .GetAsync(request.FileId, cancellationToken));
            var destination = await _fileMover
                .MoveFileAsync(file, _provider, cancellationToken);

            // Remove it from seen and add it to working
            var newFileLocation = file with {Path = destination};
        
            await Task.WhenAll(
                _erroredStore.AddAsync(
                    newFileLocation.Id,
                    newFileLocation,
                    cancellationToken),
                _workingStore.RemoveAsync(
                    file.Id,
                    cancellationToken));
            await _publisher.Publish(
                new ErroredFileMovedNotification {FileRecord = newFileLocation},
                cancellationToken);
            
            _logger.LogError(ex, "An error occurred while processing {File}.", file);
            
            throw; 
        }
    }
}