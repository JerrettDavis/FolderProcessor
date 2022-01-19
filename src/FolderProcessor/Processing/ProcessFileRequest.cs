using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Stores;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing;

[PublicAPI]
public class ProcessFileRequest : IRequest, IProcessFileRequest
{
    public Guid FileId { get; set; }
}

[UsedImplicitly]
public class ProcessFileRequestHandler : IRequestHandler<ProcessFileRequest>
{
    private readonly IWorkingFileStore _fileStore;
    private readonly IEnumerable<IProcessor> _processors;
    private readonly ILogger<ProcessFileRequestHandler> _logger;

    public ProcessFileRequestHandler(
        IWorkingFileStore fileStore, 
        IEnumerable<IProcessor> processors, 
        ILogger<ProcessFileRequestHandler> logger)
    {
        _fileStore = fileStore;
        _processors = processors;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ProcessFileRequest request,
        CancellationToken cancellationToken)
    {
        var file = await _fileStore.GetAsync(request.FileId, cancellationToken);
        _logger.LogInformation("Houston, we have liftoff! {File}", file);
        
        return Unit.Value;
    }
}