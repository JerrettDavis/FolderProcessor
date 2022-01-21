using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Processing.Behaviors;

/// <summary>
/// This behaviors moves files to a dedicated working directory before running
/// the process file request.
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">the type of the response</typeparam>
[UsedImplicitly]
public class WorkingFileMovingBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, IProcessFileRequest
{
    private readonly IWorkingFileStore _workingFileStore;
    private readonly ISeenFileStore _seenFileStore;
    private readonly IWorkingDirectoryProvider _workingDirectoryProvider;
    private readonly IFileMover _fileMover;

    public WorkingFileMovingBehavior(
        ISeenFileStore seenFileStore,
        IWorkingFileStore workingFileStore,
        IWorkingDirectoryProvider workingDirectoryProvider,
        IFileMover fileMover)
    {
        _seenFileStore = seenFileStore;
        _workingFileStore = workingFileStore;
        _workingDirectoryProvider = workingDirectoryProvider;
        _fileMover = fileMover;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        // Get the file and where to send it.
        var file = new FileRecord(await _seenFileStore
            .GetAsync(request.FileId, cancellationToken));
        var destination = await _fileMover
            .MoveFileAsync(file, _workingDirectoryProvider, cancellationToken);

        // Remove it from seen and add it to working
        var newFileLocation = file with {Path = destination};
        await Task.WhenAll(
            _workingFileStore.AddAsync(
                newFileLocation.Id,
                newFileLocation,
                cancellationToken),
            _seenFileStore.RemoveAsync(
                file.Id,
                cancellationToken));

        return await next();
    }
}