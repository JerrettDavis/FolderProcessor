using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Mediator;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Processing.Behaviors;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Behaviors;

/// <summary>
/// This behavior moves files to the completed directory once they have completed
/// successfully.
/// </summary>
[UsedImplicitly]
public class CompletedFileMovingBehavior : 
    IProcessingBehavior<ProcessFileRequest>
{
    private readonly IWorkingFileStore _workingStore;
    private readonly ICompletedFileStore _completedStore;
    private readonly ICompletedDirectoryProvider _provider;
    private readonly ILogger<CompletedFileMovingBehavior> _logger;
    private readonly IFileMover _fileMover;
    private readonly IPublisher _publisher;

    public CompletedFileMovingBehavior(
        IWorkingFileStore workingStore, 
        IFileMover fileMover, 
        ICompletedDirectoryProvider provider, 
        ICompletedFileStore completedStore, 
        IPublisher publisher, 
        ILogger<CompletedFileMovingBehavior> logger)
    {
        _workingStore = workingStore;
        _fileMover = fileMover;
        _provider = provider;
        _completedStore = completedStore;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<IProcessFileResult> Handle(
        ProcessFileRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate next)
    {
        var result = await next();
        if (!result.IsSuccessful)
            return result;

        try
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

            return new ProcessFileResult(newFileLocation);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Encountered an error trying to move {File}", request.FileId);

            return result;
        }
        
    }
}