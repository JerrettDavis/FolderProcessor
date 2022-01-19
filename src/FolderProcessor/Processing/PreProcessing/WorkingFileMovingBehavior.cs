using System.IO.Abstractions;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing.PreProcessing;

public class WorkingFileMovingBehavior<TRequest> :
    IRequestPreProcessor<TRequest> where TRequest : IProcessFileRequest
{
    private readonly IFileSystem _fileSystem;
    private readonly IWorkingFileStore _workingFileStore;
    private readonly ISeenFileStore _seenFileStore;
    private readonly IWorkingDirectoryProvider _workingDirectoryProvider;
    private readonly ILogger<WorkingFileMovingBehavior<TRequest>> _logger;

    public WorkingFileMovingBehavior(
        IFileSystem fileSystem, 
        ISeenFileStore seenFileStore, 
        IWorkingFileStore workingFileStore, 
        IWorkingDirectoryProvider workingDirectoryProvider, 
        ILogger<WorkingFileMovingBehavior<TRequest>> logger)
    {
        _fileSystem = fileSystem;
        _seenFileStore = seenFileStore;
        _workingFileStore = workingFileStore;
        _workingDirectoryProvider = workingDirectoryProvider;
        _logger = logger;
    }

    public async Task Process(
        TRequest request, 
        CancellationToken cancellationToken)
    {
        // Get the file and where to send it.
        var file = await _seenFileStore
            .GetAsync(request.FileId, cancellationToken) as FileRecord;
        var destination = await _workingDirectoryProvider
            .GetFileDestinationPath(file!.Path, cancellationToken);
        
        // If they match, run away
        if (string.Equals(file.Path, destination, StringComparison.InvariantCultureIgnoreCase))
            return;
        
        var destDirectory = _fileSystem.Path.GetDirectoryName(destination);
        
        // Ensure Directory exists & Move it
        _logger.LogInformation("Moving file from {Source} to {Destination}", file.Path, destination);
        _fileSystem.Directory.CreateDirectory(destDirectory);   
        _fileSystem.File.Move(file.Path, destination);

        // Remove it from seen and add it to working
        var newFileLocation = file with {Path = destination};
        await Task.WhenAll(
            _workingFileStore.AddAsync(newFileLocation.Id, newFileLocation, cancellationToken),
            _seenFileStore.RemoveAsync(file.Id, cancellationToken));
    }
}