using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Stores;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Processing;

[PublicAPI]
public class ProcessFileRequest : IProcessFileRequest
{
    public Guid FileId { get; set; }
}

[UsedImplicitly]
public class ProcessFileRequestHandler : IRequestHandler<ProcessFileRequest>
{
    private readonly IWorkingFileStore _fileStore;
    private readonly ICollection<IProcessor> _processors;
    private readonly ILogger<ProcessFileRequestHandler> _logger;

    public ProcessFileRequestHandler(
        IWorkingFileStore fileStore,
        IEnumerable<IProcessor> processors,
        ILogger<ProcessFileRequestHandler> logger)
    {
        _fileStore = fileStore;
        _processors = processors.ToList();
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ProcessFileRequest request,
        CancellationToken cancellationToken)
    {
        var file = await _fileStore
            .GetAsync(request.FileId, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Firing up processors...");

        await _processors
            .AsParallel()
            .ToAsyncEnumerable()
            .WhereAwaitWithCancellation(async (p, t) =>
                await p.AppliesAsync(file, t))
            .ForEachAwaitWithCancellationAsync(async (p, t) =>
                    await p.ProcessAsync(file, t),
                cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("{File} has been processed", file);

        return Unit.Value;
    }
}