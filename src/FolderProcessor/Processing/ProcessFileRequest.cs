using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Processing;
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
public class ProcessFileRequestHandler : 
    IRequestHandler<ProcessFileRequest, IProcessFileResult>
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

    public async Task<IProcessFileResult> Handle(
        ProcessFileRequest request,
        CancellationToken cancellationToken)
    {
        var file = await _fileStore
            .GetAsync(request.FileId, cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
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

            return new ProcessFileResult(file);
        }
        catch (AggregateException e)
        {
            var exceptions = e.Flatten().InnerExceptions;

            return new ProcessFileResult(file, false, exceptions);
        }
    }
}