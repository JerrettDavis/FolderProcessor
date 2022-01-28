using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Processing.Behaviors;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing;
using FolderProcessor.Models.Processing.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.Behaviors;

/// <summary>
/// This behavior monitors the process request for any errors. If an unhandled
/// exception occurs, the file is moved to the error directory.
/// </summary>
[UsedImplicitly]
public class ErroredFileMovingBehavior :
    IProcessingBehavior<ProcessFileRequest>
{
    private readonly IWorkingFileStore _workingStore;
    private readonly IErroredFileStore _erroredStore;
    private readonly IErroredDirectoryProvider _provider;
    private readonly IFileMover _fileMover;
    private readonly ILogger<ErroredFileMovingBehavior> _logger;
    private readonly IPublisher _publisher;

    public ErroredFileMovingBehavior(
        IWorkingFileStore workingStore,
        IErroredFileStore erroredStore,
        IFileMover fileMover,
        ILogger<ErroredFileMovingBehavior> logger,
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

    public async Task<IProcessFileResult> Handle(
        ProcessFileRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<IProcessFileResult> next)
    {
        var result = await next();
        if (result.IsSuccessful)
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

            _logger.LogError("Moved errored {File} to {Destination}.", file, destination);

            return new ProcessFileResult(newFileLocation, false, result.Errors);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Encountered an error trying to move {File}", request.FileId);
            
            var newErrors = result.Errors.ToList();
            newErrors.Add(ex);
            
            return new ProcessFileResult(result.FileRecord, false, newErrors);
        }

    }
}