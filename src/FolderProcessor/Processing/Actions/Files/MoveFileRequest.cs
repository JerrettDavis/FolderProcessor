using System.IO.Abstractions;
using FolderProcessor.Models.Common;
using MediatR;

namespace FolderProcessor.Processing.Actions.Files;

public class MoveFileRequest : IRequest
{
    /// <summary>
    /// The source path for the file being moved.
    /// </summary>
    public string Source { get; set; } = null!;
    
    /// <summary>
    /// The destination path for the file being moved.
    /// </summary>
    public string Destination { get; set; } = null!;
    
    /// <summary>
    /// The retry mechanism to use in the event of an error
    /// </summary>
    public Retry Retry { get; set; } = Retry.None;
}

public class MoveFileRequestHandler : IRequestHandler<MoveFileRequest>
{
    private readonly IFileSystem _fileSystem;

    public MoveFileRequestHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public Task<Unit> Handle(
        MoveFileRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}