using System.IO.Abstractions;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Processing.Actions.Files;

[PublicAPI]
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
}

[UsedImplicitly]
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